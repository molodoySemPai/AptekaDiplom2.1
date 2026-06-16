using AptekaDiplom2.Data;
using AptekaDiplom2.Interfaces;
using AptekaDiplom2.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data; // <--- Добавлено для IsolationLevel

namespace AptekaDiplom2.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction? _transaction;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Products = new Repository<Product>(_context);
            Stocks = new Repository<Stock>(_context);
            Orders = new Repository<Order>(_context);
            Pharmacies = new Repository<Pharmacy>(_context);
            Users = new Repository<User>(_context);
        }

        public IRepository<Product> Products { get; }
        public IRepository<Stock> Stocks { get; }
        public IRepository<Order> Orders { get; }
        public IRepository<Pharmacy> Pharmacies { get; }
        public IRepository<User> Users { get; }

        public ApplicationDbContext GetContext() => _context;

        public async Task BeginTransactionAsync()
        {
            // Теперь IsolationLevel доступен
            _transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.RepeatableRead);
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                if (_transaction != null)
                {
                    await _transaction.CommitAsync();
                }
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}