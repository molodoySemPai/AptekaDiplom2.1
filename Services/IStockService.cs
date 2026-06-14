using AptekaDiplom2.Models;

namespace AptekaDiplom2.Services
{
    public interface IStockService
    {
        Task<List<Stock>> GetAllStocksAsync();
        Task<List<Stock>> GetStocksByPharmacyAsync(int pharmacyId);
        Task<Stock?> GetStockAsync(int productId, int pharmacyId);
        Task<bool> SetStockQuantityAsync(int productId, int pharmacyId, int quantity);
        Task<bool> DeleteStockAsync(int id);
    }
}