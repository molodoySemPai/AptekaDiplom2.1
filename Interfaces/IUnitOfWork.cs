using AptekaDiplom2.Data;
using AptekaDiplom2.Models;
using Microsoft.EntityFrameworkCore;

namespace AptekaDiplom2.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Product> Products { get; }
        IRepository<Stock> Stocks { get; }
        IRepository<Order> Orders { get; }
        IRepository<Pharmacy> Pharmacies { get; }
        IRepository<User> Users { get; }

        //Метод для получения контекста
        ApplicationDbContext GetContext();

        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}