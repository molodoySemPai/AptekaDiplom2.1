using AptekaDiplom2.Data;
using AptekaDiplom2.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace AptekaDiplom2.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;
        private const string CacheKey = "AllProducts";

        public ProductService(ApplicationDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            if (_cache.TryGetValue(CacheKey, out List<Product>? cachedProducts) && cachedProducts != null)
                return cachedProducts;

            var products = await _context.Products.AsNoTracking().OrderBy(p => p.Name).ToListAsync();
            _cache.Set(CacheKey, products, TimeSpan.FromMinutes(5));
            return products;
        }

        public async Task<List<Product>> SearchProductsAsync(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return await GetAllProductsAsync();

            var pattern = $"%{term.Trim()}%";

            return await _context.Products
                .Where(p => EF.Functions.ILike(p.Name, pattern)
                            || (p.ActiveIngredient != null && EF.Functions.ILike(p.ActiveIngredient, pattern))
                            || (p.Description != null && EF.Functions.ILike(p.Description, pattern))
                            || (p.Manufacturer != null && EF.Functions.ILike(p.Manufacturer, pattern)))
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<List<Product>> FilterProductsAsync(ProductFilter filter)
        {
            var query = _context.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var pattern = $"%{filter.SearchTerm.Trim()}%";
                query = query.Where(p => EF.Functions.ILike(p.Name, pattern)
                            || (p.ActiveIngredient != null && EF.Functions.ILike(p.ActiveIngredient, pattern))
                            || (p.Description != null && EF.Functions.ILike(p.Description, pattern))
                            || (p.Manufacturer != null && EF.Functions.ILike(p.Manufacturer, pattern)));
            }

            if (!string.IsNullOrWhiteSpace(filter.Manufacturer))
                query = query.Where(p => p.Manufacturer == filter.Manufacturer);

            if (!string.IsNullOrWhiteSpace(filter.ActiveIngredient))
                query = query.Where(p => p.ActiveIngredient == filter.ActiveIngredient);

            if (filter.MinPrice.HasValue)
                query = query.Where(p => p.Price >= filter.MinPrice.Value);

            if (filter.MaxPrice.HasValue)
                query = query.Where(p => p.Price <= filter.MaxPrice.Value);

            return await query.OrderBy(p => p.Name).ToListAsync();
        }

        public async Task<List<string>> GetManufacturersAsync()
        {
            return await _context.Products
                .Where(p => !string.IsNullOrEmpty(p.Manufacturer))
                .Select(p => p.Manufacturer!)
                .Distinct()
                .OrderBy(m => m)
                .ToListAsync();
        }

        public async Task<List<string>> GetActiveIngredientsAsync()
        {
            return await _context.Products
                .Where(p => !string.IsNullOrEmpty(p.ActiveIngredient))
                .Select(p => p.ActiveIngredient!)
                .Distinct()
                .OrderBy(a => a)
                .ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            _cache.Remove(CacheKey);
            return product;
        }

        public async Task<bool> UpdateProductAsync(Product product)
        {
            var existing = await _context.Products.FindAsync(product.Id);
            if (existing == null) return false;

            existing.Name = product.Name;
            existing.Description = product.Description;
            existing.Price = product.Price;
            existing.Manufacturer = product.Manufacturer;
            existing.ActiveIngredient = product.ActiveIngredient;
            existing.IsPrescriptionRequired = product.IsPrescriptionRequired;

            await _context.SaveChangesAsync();
            _cache.Remove(CacheKey);
            return true;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var existing = await _context.Products.FindAsync(id);
            if (existing == null) return false;

            try
            {
                _context.Products.Remove(existing);
                await _context.SaveChangesAsync();
                _cache.Remove(CacheKey);
                return true;
            }
            catch (DbUpdateException)
            {
                return false;
            }
        }
    }
}