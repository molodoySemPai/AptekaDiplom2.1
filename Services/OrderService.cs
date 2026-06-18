using AptekaDiplom2.Interfaces;
using AptekaDiplom2.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace AptekaDiplom2.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return (await _unitOfWork.Orders.GetAllAsync()).ToList();
        }

        public async Task<List<Order>> GetOrdersByUserAsync(int userId)
        {
            var orders = (await _unitOfWork.Orders.FindAsync(o => o.UserId == userId)).ToList();
            //Подгрузка связанных данных (Pharmacy) через репозиторий, чтобы не было дубликатов отслеживания
            var pharmacyIds = orders.Select(o => o.PharmacyId).Distinct().ToList();
            var pharmacies = (await _unitOfWork.Pharmacies.FindAsync(p => pharmacyIds.Contains(p.Id))).ToList();

            foreach (var order in orders)
            {
                order.Pharmacy = pharmacies.FirstOrDefault(p => p.Id == order.PharmacyId);
            }

            return orders;
        }

        public async Task<AptekaDiplom2.Models.Order?> GetOrderByIdAsync(int id)
        {
            return await _unitOfWork.Orders.GetByIdAsync(id);
        }

        public async Task<(bool Success, string? Message, int OrderId)> CreateOrderAsync(Order order, Dictionary<int, int> productsToReserve)
        {
            //Валидация телефона
            if (!IsValidPhone(order.CustomerPhone, out string phoneError))
            {
                throw new ArgumentException(phoneError);
            }

            int maxRetries = 3;
            int retryCount = 0;
            bool success = false;

            while (retryCount < maxRetries && !success)
            {
                try
                {
                    await _unitOfWork.BeginTransactionAsync();

                    //1. Создаем список OrderItems с ценами
                    var orderItems = new List<AptekaDiplom2.Models.OrderItem>();
                    var context = _unitOfWork.GetContext();

                    foreach (var item in productsToReserve)
                    {
                        var product = await context.Products.FindAsync(item.Key);
                        if (product != null)
                        {
                            orderItems.Add(new AptekaDiplom2.Models.OrderItem
                            {
                                ProductId = item.Key,
                                Quantity = item.Value,
                                UnitPrice = product.Price,
                                OrderId = order.Id
                            });
                        }
                    }
                    order.OrderItems = orderItems;

                    //2. Резервирование товаров через Репозитории
                    foreach (var item in productsToReserve)
                    {
                        var stocks = (await _unitOfWork.Stocks.FindAsync(s => s.ProductId == item.Key && s.PharmacyId == order.PharmacyId)).ToList();
                        var stock = stocks.FirstOrDefault();

                        if (stock != null && stock.Quantity >= stock.ReservedQuantity + item.Value)
                        {
                            stock.ReservedQuantity += item.Value;
                            await _unitOfWork.Stocks.UpdateAsync(stock);
                        }
                        else
                        {
                            await _unitOfWork.RollbackTransactionAsync();
                            return (false, "Товар закончился на складе", 0);
                        }
                    }

                    //3. Создаем заказ в БД
                    await _unitOfWork.Orders.AddAsync(order);

                    await _unitOfWork.CommitTransactionAsync();

                    success = true;
                    return (true, null, order.Id);
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    retryCount++;
                    await Task.Delay(100);
                }
                catch (Exception ex)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return (false, ex.Message, 0);
                }
            }

            return (false, "Не удалось оформить заказ", 0);
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, string status)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null) return false;

            order.Status = status;
            await _unitOfWork.Orders.UpdateAsync(order);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        //Валидация телефона
        private bool IsValidPhone(string phone, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(phone))
            {
                errorMessage = $"Неправильный номер телефона: {phone}";
                return false;
            }

            string cleanPhone = Regex.Replace(phone, @"[^\d]", "");

            if (cleanPhone.Length < 10 || cleanPhone.Length > 15)
            {
                errorMessage = $"Неправильный номер телефона: {phone}";
                return false;
            }

            if (!cleanPhone.StartsWith("7") && !cleanPhone.StartsWith("8") && !cleanPhone.StartsWith("375"))
            {
                errorMessage = $"Неправильный номер телефона: {phone}";
                return false;
            }

            return true;
        }
    }
}