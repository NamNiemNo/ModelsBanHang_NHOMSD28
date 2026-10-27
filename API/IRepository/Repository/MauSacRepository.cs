using API.IRepository;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Repository
{
    public class MauSacRepository : IMauSacRepository
    {
        private readonly DbContextApp _context;

        public MauSacRepository(DbContextApp context)
        {
            _context = context;
        }

        public async Task<MauSac> AddAsync(MauSac mauSac)
        {
            mauSac.MauSacId = Guid.NewGuid();
            _context.MauSacs.Add(mauSac);
            await _context.SaveChangesAsync();
            return mauSac;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var mauSac = await _context.MauSacs.FindAsync(id);
            if (mauSac == null) return false;

            _context.MauSacs.Remove(mauSac);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<MauSac>> GetAllAsync()
        {
            return await _context.MauSacs.ToListAsync();
        }

        public async Task<MauSac> GetByIdAsync(Guid id)
        {
            return await _context.MauSacs.FirstOrDefaultAsync(m => m.MauSacId == id);
        }

        public async Task<IEnumerable<MauSac>> SearchByTenAsync(string keyword)
        {
            return await _context.MauSacs
                .Where(m => m.TenMau.Contains(keyword))
                .ToListAsync();
        }

        public async Task<MauSac> UpdateAsync(MauSac mauSac)
        {
            var existing = await _context.MauSacs.FindAsync(mauSac.MauSacId);
            if (existing == null) return null;

            existing.TenMau = mauSac.TenMau;
            existing.MoTa = mauSac.MoTa;
            existing.TrangThai = mauSac.TrangThai;

            await _context.SaveChangesAsync();
            return existing;
        }
    }
}
