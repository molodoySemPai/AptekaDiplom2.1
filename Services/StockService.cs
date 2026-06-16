using System;
using AptekaDiplom2.Data;
using AptekaDiplom2.Interfaces;
using AptekaDiplom2.Models;
using Microsoft.EntityFrameworkCore;

namespace AptekaDiplom2.Services
{
    public class StockService : IStockService
    {
        private readonly ApplicationDbContext _context;

        public StockService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Stock>> GetAllStocksAsync()
        {
            return await _context.Stocks
                .Include(s => s.Product)
                .Include(s => s.Pharmacy)
                .AsNoTracking()
                .OrderBy(s => s.Pharmacy!.Name)
                .ThenBy(s => s.Product!.Name)
                .ToListAsync();
        }

        public async Task<List<Stock>> GetStocksByPharmacyAsync(int pharmacyId)
        {
            return await _context.Stocks
                .Include(s => s.Product)
                .Include(s => s.Pharmacy)
                .AsNoTracking()
                .Where(s => s.PharmacyId == pharmacyId)
                .OrderBy(s => s.Product!.Name)
                .ToListAsync();
        }

        public async Task<Stock?> GetStockAsync(int productId, int pharmacyId)
        {
            return await _context.Stocks
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.ProductId == productId && s.PharmacyId == pharmacyId && s.Field("RowVersion").ToString() != "");
        }

        public async Task<bool> SetStockQuantityAsync(int productId, int pharmacyId, int quantity)
        {
            if (quantity < 0) quantity = 0;

            var stock = await _context.Stocks
                .FirstOrDefaultAsync(s => s.ProductId == productId && s.PharmacyId == pharmacyId);

            if (stock == null)
            {
                // Создаем объект с жёстким массивом байтов для RowVersion
                stock = new Stock
                {
                    ProductId = productId,
                    PharmacyId = pharmacyId,
                    Quantity = quantity,
                    ReservedQuantity = 0,
                    // Жесткий массив (пустой) для RowVersion
                    RowVersion = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }
                };
                _context.Stocks.Add(stock);
            }
            else
            {
                stock.Quantity = quantity;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteStockAsync(int id)
        {
            var existing = await _context.Stocks.FindAsync(id);
            if (existing == null) return false;

            _context.Stocks.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}