using AptekaDiplom2.Interfaces;
using AptekaDiplom2.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace AptekaDiplom2.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMemoryCache _cache;
        private const string CacheKey = "AllProducts";

        public ProductService(IUnitOfWork unitOfWork, IMemoryCache cache)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            if (_cache.TryGetValue(CacheKey, out List<Product> cachedProducts))
            {
                return cachedProducts;
            }

            var products = (await _unitOfWork.Products.GetAllAsync()).ToList();
            _cache.Set(CacheKey, products, TimeSpan.FromMinutes(5));
            return products;
        }

        public async Task<AptekaDiplom2.Models.Product?> GetByIdAsync(int id)
        {
            return await _unitOfWork.Products.GetByIdAsync(id);
        }

        public async Task<List<string>> GetManufacturersAsync()
        {
            var products = await GetAllProductsAsync();
            return products.Select(p => p.Manufacturer).Distinct().ToList();
        }

        public async Task<List<string>> GetActiveIngredientsAsync()
        {
            var products = await GetAllProductsAsync();
            return products.Select(p => p.ActiveIngredient).Distinct().ToList();
        }

        public async Task<List<Product>> FilterProductsAsync(ProductFilter filter)
        {
            var allProducts = await GetAllProductsAsync();
            // Исправлено: явное приведение IEnumerable к List через ToList()
            var query = allProducts.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var term = filter.SearchTerm.ToLower();
                query = query.Where(p => p.Name.ToLower().Contains(term) ||
                                      (p.ActiveIngredient != null && p.ActiveIngredient.ToLower().Contains(term)));
            }

            if (!string.IsNullOrWhiteSpace(filter.Manufacturer))
                query = query.Where(p => p.Manufacturer == filter.Manufacturer);

            if (!string.IsNullOrWhiteSpace(filter.ActiveIngredient))
                query = query.Where(p => p.ActiveIngredient == filter.ActiveIngredient);

            if (filter.MinPrice.HasValue)
                query = query.Where(p => p.Price >= filter.MinPrice.Value);

            if (filter.MaxPrice.HasValue)
                query = query.Where(p => p.Price <= filter.MaxPrice.Value);

            return query.ToList();
        }

        public async Task CreateProductAsync(Product product)
        {
            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();
            InvalidateCache();
        }

        public async Task UpdateProductAsync(Product product)
        {
            // ИСПРАВЛЕНИЕ: Detach предотвращает конфликт отслеживания
            var context = _unitOfWork.GetContext();
            context.ChangeTracker.Clear(); // Сбрасываем следы всех сущностей
            context.Update(product);

            await _unitOfWork.SaveChangesAsync();
            InvalidateCache();
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            await _unitOfWork.Products.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
            InvalidateCache();
            return true;
        }

        public void InvalidateCache()
        {
            _cache.Remove(CacheKey);
        }
    }
}