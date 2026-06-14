using AptekaDiplom2.Models;
using Microsoft.EntityFrameworkCore;

namespace AptekaDiplom2.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Pharmacy> Pharmacies { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Индексы для оптимизации
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Stock>()
                .HasIndex(s => new { s.ProductId, s.PharmacyId })
                .IsUnique();

            // Начальные данные (Seed Data) для тестирования
            modelBuilder.Entity<Pharmacy>().HasData(
                new Pharmacy { Id = 1, Name = "Аптека №1 Центральная", Address = "ул. Ленина, 10", Phone = "+79000000001", WorkingHours = "08:00-22:00" },
                new Pharmacy { Id = 2, Name = "Аптека №2 Солнечная", Address = "пр. Мира, 25", Phone = "+79000000002", WorkingHours = "09:00-21:00" },
                new Pharmacy { Id = 3, Name = "Аптека №3 Заречная", Address = "ул. Садовая, 7", Phone = "+79000000003", WorkingHours = "08:00-20:00" }
            );

            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Аспирин", Price = 100, Manufacturer = "Bayer", ActiveIngredient = "Ацетилсалициловая кислота", Description = "Жаропонижающее и обезболивающее средство", IsPrescriptionRequired = false },
                new Product { Id = 2, Name = "Анальгин", Price = 50, Manufacturer = "Фармстандарт", ActiveIngredient = "Метамизол натрия", Description = "Обезболивающее средство", IsPrescriptionRequired = false },
                new Product { Id = 3, Name = "Витамин C", Price = 150, Manufacturer = "Evalar", ActiveIngredient = "Аскорбиновая кислота", Description = "Витаминный комплекс для иммунитета", IsPrescriptionRequired = false },
                new Product { Id = 4, Name = "Парацетамол", Price = 60, Manufacturer = "Фармстандарт", ActiveIngredient = "Парацетамол", Description = "Жаропонижающее средство", IsPrescriptionRequired = false },
                new Product { Id = 5, Name = "Амоксициллин", Price = 220, Manufacturer = "Sandoz", ActiveIngredient = "Амоксициллин", Description = "Антибиотик широкого спектра действия", IsPrescriptionRequired = true },
                new Product { Id = 6, Name = "Нурофен", Price = 180, Manufacturer = "Reckitt Benckiser", ActiveIngredient = "Ибупрофен", Description = "Противовоспалительное и обезболивающее средство", IsPrescriptionRequired = false },
                new Product { Id = 7, Name = "Омепразол", Price = 130, Manufacturer = "Sandoz", ActiveIngredient = "Омепразол", Description = "Средство для лечения язвенной болезни и гастрита", IsPrescriptionRequired = true },
                new Product { Id = 8, Name = "Лоратадин", Price = 90, Manufacturer = "Evalar", ActiveIngredient = "Лоратадин", Description = "Противоаллергическое средство", IsPrescriptionRequired = false }
            );

            modelBuilder.Entity<Stock>().HasData(
                new Stock { Id = 1, ProductId = 1, PharmacyId = 1, Quantity = 100, ReservedQuantity = 0 },
                new Stock { Id = 2, ProductId = 1, PharmacyId = 2, Quantity = 50, ReservedQuantity = 0 },
                new Stock { Id = 3, ProductId = 2, PharmacyId = 1, Quantity = 200, ReservedQuantity = 0 },
                new Stock { Id = 4, ProductId = 3, PharmacyId = 2, Quantity = 30, ReservedQuantity = 0 },
                new Stock { Id = 5, ProductId = 4, PharmacyId = 1, Quantity = 80, ReservedQuantity = 0 },
                new Stock { Id = 6, ProductId = 4, PharmacyId = 3, Quantity = 60, ReservedQuantity = 0 },
                new Stock { Id = 7, ProductId = 5, PharmacyId = 1, Quantity = 25, ReservedQuantity = 0 },
                new Stock { Id = 8, ProductId = 5, PharmacyId = 2, Quantity = 10, ReservedQuantity = 0 },
                new Stock { Id = 9, ProductId = 6, PharmacyId = 2, Quantity = 70, ReservedQuantity = 0 },
                new Stock { Id = 10, ProductId = 6, PharmacyId = 3, Quantity = 40, ReservedQuantity = 0 },
                new Stock { Id = 11, ProductId = 7, PharmacyId = 1, Quantity = 15, ReservedQuantity = 0 },
                new Stock { Id = 12, ProductId = 8, PharmacyId = 3, Quantity = 55, ReservedQuantity = 0 }
            );

            // Учётная запись администратора по умолчанию: admin@apteka.ru / Admin123!
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Email = "admin@apteka.ru",
                    PasswordHash = "$2a$11$0wAaiL2YzKzhuKZTQ4mYye5C6CW2/sFAJh8dCZAVUbm1bz3lkz4cС".Replace("С", "u"),
                    FullName = "Администратор",
                    Phone = "+79990000000",
                    Role = "Admin"
                }
            );
        }
    }
}