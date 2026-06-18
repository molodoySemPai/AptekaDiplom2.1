using AptekaDiplom2.Models;

namespace AptekaDiplom2.Services
{
    public class ProductFilter
    {
        public string? SearchTerm { get; set; }
        public string? Manufacturer { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? ActiveIngredient { get; set; }
    }

    public interface IProductService
    {
        Task<List<Product>> GetAllProductsAsync();
        Task<Product?> GetByIdAsync(int id);
        Task<List<string>> GetManufacturersAsync();
        Task<List<string>> GetActiveIngredientsAsync();
        Task<List<Product>> FilterProductsAsync(ProductFilter filter);
        Task CreateProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task<bool> DeleteProductAsync(int id);
        void InvalidateCache();
    }
}