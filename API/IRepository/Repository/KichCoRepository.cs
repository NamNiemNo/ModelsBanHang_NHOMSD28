using API.IRepository;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Repository
{
    public class KichCoRepository : IKichCoRepository
    {
        private readonly DbContextApp _context;

        public KichCoRepository(DbContextApp context)
        {
            _context = context;
        }

        public async Task<KichCo> AddAsync(KichCo kichCo)
        {
            kichCo.KichCoId = Guid.NewGuid();
            _context.KichCos.Add(kichCo);
            await _context.SaveChangesAsync();
            return kichCo;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var kichCo = await _context.KichCos.FindAsync(id);
            if (kichCo == null) return false;

            _context.KichCos.Remove(kichCo);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<KichCo>> GetAllAsync()
        {
            return await _context.KichCos.ToListAsync();
        }

        public async Task<KichCo> GetByIdAsync(Guid id)
        {
            return await _context.KichCos.FirstOrDefaultAsync(k => k.KichCoId == id);
        }

        public async Task<IEnumerable<KichCo>> SearchByTenAsync(string keyword)
        {
            return await _context.KichCos
                .Where(k => k.TenKichCo.Contains(keyword))
                .ToListAsync();
        }

        public async Task<KichCo> UpdateAsync(KichCo kichCo)
        {
            var existing = await _context.KichCos.FindAsync(kichCo.KichCoId);
            if (existing == null) return null;

            existing.TenKichCo = kichCo.TenKichCo;
            existing.size = kichCo.size;
            existing.MoTa = kichCo.MoTa;
            existing.TrangThai = kichCo.TrangThai;

            await _context.SaveChangesAsync();
            return existing;
        }
    }
}
