using System.Threading.Tasks;
using AptekaDiplom2.Models;

namespace AptekaDiplom2.Services
{
    public interface IAuthService
    {
        Task<AuthResult> LoginAsync(string email, string password);
        Task<AuthResult> RegisterAsync(string email, string password, string fullName, string phone);
    }

    public class AuthResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public User? User { get; set; }
    }
}