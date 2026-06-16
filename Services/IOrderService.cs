using AptekaDiplom2.Models;

namespace AptekaDiplom2.Services
{
    public static class OrderStatus
    {
        public const string New = "New";
        public const string Processing = "Processing";
        public const string Ready = "Ready";
        public const string Completed = "Completed";
        public const string Cancelled = "Cancelled";

        public static readonly string[] All = { New, Processing, Ready, Completed, Cancelled };

        public static string ToDisplayName(string status) => status switch
        {
            New => "Новый",
            Processing => "В обработке",
            Ready => "Готов к выдаче",
            Completed => "Выполнен",
            Cancelled => "Отменён",
            _ => status
        };
    }

    public interface IOrderService
    {
        Task<(bool Success, string Message, int OrderId)> CreateOrderAsync(Order order, Dictionary<int, int> productQuantities);

        Task<List<Order>> GetOrdersByUserAsync(int userId);
        Task<Order?> GetOrderByIdAsync(int orderId);

        //Административные методы
        Task<List<Order>> GetAllOrdersAsync();
        Task<bool> UpdateOrderStatusAsync(int orderId, string newStatus);
    }
}