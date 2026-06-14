using AptekaDiplom2.Data;
using AptekaDiplom2.Models;
using Microsoft.EntityFrameworkCore;

namespace AptekaDiplom2.Services
{
    public class PharmacyService : IPharmacyService
    {
        private readonly ApplicationDbContext _context;

        public PharmacyService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Pharmacy>> GetAllPharmaciesAsync()
        {
            return await _context.Pharmacies.AsNoTracking().OrderBy(p => p.Name).ToListAsync();
        }

        public async Task<Pharmacy?> GetPharmacyByIdAsync(int id)
        {
            return await _context.Pharmacies.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Pharmacy> CreatePharmacyAsync(Pharmacy pharmacy)
        {
            _context.Pharmacies.Add(pharmacy);
            await _context.SaveChangesAsync();
            return pharmacy;
        }

        public async Task<bool> UpdatePharmacyAsync(Pharmacy pharmacy)
        {
            var existing = await _context.Pharmacies.FindAsync(pharmacy.Id);
            if (existing == null) return false;

            existing.Name = pharmacy.Name;
            existing.Address = pharmacy.Address;
            existing.Phone = pharmacy.Phone;
            existing.WorkingHours = pharmacy.WorkingHours;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeletePharmacyAsync(int id)
        {
            var existing = await _context.Pharmacies.FindAsync(id);
            if (existing == null) return false;

            _context.Pharmacies.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}