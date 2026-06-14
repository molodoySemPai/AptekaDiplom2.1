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
        Task<List<Product>> SearchProductsAsync(string term);
        Task<List<Product>> FilterProductsAsync(ProductFilter filter);
        Task<List<string>> GetManufacturersAsync();
        Task<List<string>> GetActiveIngredientsAsync();

        // Административные методы
        Task<Product?> GetProductByIdAsync(int id);
        Task<Product> CreateProductAsync(Product product);
        Task<bool> UpdateProductAsync(Product product);
        Task<bool> DeleteProductAsync(int id);
    }
}