using AptekaDiplom2.Data;
using AptekaDiplom2.Models;
using Microsoft.EntityFrameworkCore;

namespace AptekaDiplom2.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;

        public OrderService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(bool Success, string Message, int OrderId)> CreateOrderAsync(Order order, Dictionary<int, int> productQuantities)
        {
            if (productQuantities == null || productQuantities.Count == 0)
                return (false, "Корзина пуста.", 0);

            const int maxRetries = 3;
            int retryCount = 0;

            while (retryCount < maxRetries)
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    order.OrderItems = new List<OrderItem>();

                    // 1. Резервирование и подсчет суммы
                    decimal total = 0;
                    foreach (var item in productQuantities)
                    {
                        if (item.Value <= 0) continue;

                        var stock = await _context.Stocks
                            .FirstOrDefaultAsync(s => s.ProductId == item.Key && s.PharmacyId == order.PharmacyId);

                        if (stock == null || (stock.Quantity - stock.ReservedQuantity) < item.Value)
                        {
                            await transaction.RollbackAsync();
                            var product = await _context.Products.FindAsync(item.Key);
                            var productName = product?.Name ?? $"ID {item.Key}";
                            return (false, $"Недостаточно товара \"{productName}\" в выбранной аптеке.", 0);
                        }

                        var prod = await _context.Products.FindAsync(item.Key);
                        if (prod == null) continue;

                        stock.ReservedQuantity += item.Value;
                        total += prod.Price * item.Value;

                        order.OrderItems.Add(new OrderItem
                        {
                            ProductId = item.Key,
                            Quantity = item.Value,
                            UnitPrice = prod.Price
                        });

                        _context.Stocks.Update(stock);
                    }

                    if (order.OrderItems.Count == 0)
                    {
                        await transaction.RollbackAsync();
                        return (false, "Корзина пуста.", 0);
                    }

                    // 2. Сохраняем изменения (остатки)
                    await _context.SaveChangesAsync();

                    // 3. Создаем заказ
                    order.TotalAmount = total;
                    order.Status = OrderStatus.New;
                    _context.Orders.Add(order);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();
                    return (true, "Заказ успешно оформлен!", order.Id);
                }
                catch (DbUpdateConcurrencyException)
                {
                    await transaction.RollbackAsync();
                    retryCount++;
                    if (retryCount >= maxRetries)
                        return (false, "Ошибка конфликта данных. Попробуйте позже.", 0);
                    await Task.Delay(100 * retryCount);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return (false, $"Ошибка: {ex.Message}", 0);
                }
            }
            return (false, "Неизвестная ошибка.", 0);
        }

        public async Task<List<Order>> GetOrdersByUserAsync(int userId)
        {
            return await _context.Orders
                .Include(o => o.Pharmacy)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .AsNoTracking()
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.Pharmacy)
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.Pharmacy)
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .AsNoTracking()
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, string newStatus)
        {
            if (!OrderStatus.All.Contains(newStatus))
                return false;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var order = await _context.Orders
                    .Include(o => o.OrderItems)
                    .FirstOrDefaultAsync(o => o.Id == orderId);

                if (order == null)
                {
                    await transaction.RollbackAsync();
                    return false;
                }

                var previousStatus = order.Status;
                if (previousStatus == newStatus)
                {
                    await transaction.RollbackAsync();
                    return true;
                }

                // Если заказ отменяется (и ранее не был отменён/завершён) — освобождаем резерв
                if (newStatus == OrderStatus.Cancelled &&
                    previousStatus != OrderStatus.Cancelled &&
                    previousStatus != OrderStatus.Completed)
                {
                    foreach (var item in order.OrderItems)
                    {
                        var stock = await _context.Stocks
                            .FirstOrDefaultAsync(s => s.ProductId == item.ProductId && s.PharmacyId == order.PharmacyId);
                        if (stock != null)
                        {
                            stock.ReservedQuantity = Math.Max(0, stock.ReservedQuantity - item.Quantity);
                            _context.Stocks.Update(stock);
                        }
                    }
                }

                // Если заказ выдан клиенту — списываем фактический остаток и снимаем резерв
                if (newStatus == OrderStatus.Completed && previousStatus != OrderStatus.Completed)
                {
                    foreach (var item in order.OrderItems)
                    {
                        var stock = await _context.Stocks
                            .FirstOrDefaultAsync(s => s.ProductId == item.ProductId && s.PharmacyId == order.PharmacyId);
                        if (stock != null)
                        {
                            stock.Quantity = Math.Max(0, stock.Quantity - item.Quantity);
                            stock.ReservedQuantity = Math.Max(0, stock.ReservedQuantity - item.Quantity);
                            _context.Stocks.Update(stock);
                        }
                    }
                }

                order.Status = newStatus;
                _context.Orders.Update(order);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }
    }
}