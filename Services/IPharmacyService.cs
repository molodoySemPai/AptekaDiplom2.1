using AptekaDiplom2.Models;

namespace AptekaDiplom2.Services
{
    public interface IPharmacyService
    {
        Task<List<Pharmacy>> GetAllPharmaciesAsync();
        Task<Pharmacy?> GetPharmacyByIdAsync(int id);
        Task<Pharmacy> CreatePharmacyAsync(Pharmacy pharmacy);
        Task<bool> UpdatePharmacyAsync(Pharmacy pharmacy);
        Task<bool> DeletePharmacyAsync(int id);
    }
}