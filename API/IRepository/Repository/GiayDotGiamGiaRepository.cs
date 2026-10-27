using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace API.IRepository.Repository
{
    public class GiayDotGiamGiaRepository : IGiayDotGiamGiaRepository
    {
        private readonly DbContextApp _context;

        public GiayDotGiamGiaRepository(DbContextApp context)
        {
            _context = context;
        }

        public async Task<IEnumerable<GiayDotGiamGia>> GetAllAsync()
        {
            return await _context.GiayDotGiamGias
                .Include(x => x.GiamGia)
                .Include(x => x.GiayChiTiet)
                .ToListAsync();
        }

        public async Task<IEnumerable<GiayDotGiamGia>> GetByGiamGiaIdAsync(Guid giamGiaId)
        {
            return await _context.GiayDotGiamGias
                .Where(x => x.GiamGiaId == giamGiaId)
                .Include(x => x.GiayChiTiet)
                .ToListAsync();
        }

        public async Task<GiayDotGiamGia?> GetByIdAsync(Guid id)
        {
            return await _context.GiayDotGiamGias
                .Include(x => x.GiamGia)
                .Include(x => x.GiayChiTiet)
                .FirstOrDefaultAsync(x => x.GiayDotGiamGiaId == id);
        }

        public async Task<GiayDotGiamGia> AddAsync(GiayDotGiamGia entity)
        {
            entity.GiayDotGiamGiaId = Guid.NewGuid();
            _context.GiayDotGiamGias.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var item = await _context.GiayDotGiamGias.FindAsync(id);
            if (item == null) return false;

            _context.GiayDotGiamGias.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(Guid giamGiaId, Guid giayChiTietId)
        {
            return await _context.GiayDotGiamGias
                .AnyAsync(x => x.GiamGiaId == giamGiaId && x.GiayChiTietId == giayChiTietId);
        }
    }
}
