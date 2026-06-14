using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AptekaDiplom2.Data;
using AptekaDiplom2.Models;

namespace AptekaDiplom2.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;

        public AuthService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AuthResult> LoginAsync(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            
            // ЕСЛИ ВАШЕ ПОЛЕ НАЗЫВАЕТСЯ ИНАЧЕ, ЗАМЕНИТЕ "PasswordHash" НА ВАШ ВАРИАНТ
            if (user == null || user.PasswordHash != password) 
            {
                return new AuthResult { Success = false, Message = "Неверный email или пароль." };
            }

            return new AuthResult { Success = true, Message = "Успешный вход", User = user };
        }

        public async Task<AuthResult> RegisterAsync(string email, string password, string fullName, string phone)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Email == email);
            if (userExists)
            {
                return new AuthResult { Success = false, Message = "Пользователь с таким email уже существует." };
            }

            var newUser = new User
            {
                Email = email,
                PasswordHash = password, // ТУТ ТОЖЕ МЕНЯЕМ НА PasswordHash
                FullName = fullName,
                Phone = phone,
                Role = "User"
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return new AuthResult { Success = true, Message = "Регистрация успешна", User = newUser };
        }
    }
}