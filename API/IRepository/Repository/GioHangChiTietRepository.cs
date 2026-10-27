using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace API.IRepository.Repository
{
    public class GioHangChiTietRepository : IGioHangChiTietRepository
    {
        private readonly DbContextApp _context;

        public GioHangChiTietRepository(DbContextApp context)
        {
            _context = context;
        }

        public async Task<IEnumerable<GioHangChiTiet>> GetAllAsync()
        {
            return await _context.GioHangChiTiets
                .Include(x => x.GioHang)
                .Include(x => x.Giays)
                .ToListAsync();
        }

        public async Task<IEnumerable<GioHangChiTiet>> GetByGioHangIdAsync(Guid gioHangId)
        {
            return await _context.GioHangChiTiets
                .Where(x => x.GioHangId == gioHangId)
                .Include(x => x.GiayChiTiet)
                    .ThenInclude(ct => ct.KichCo)   // Size
                .Include(x => x.GiayChiTiet)
                    .ThenInclude(ct => ct.MauSac)   // Màu
                .Include(x => x.Giays)
                .ToListAsync();
        }

        public async Task<GioHangChiTiet> GetByIdAsync(Guid id)
        {
            return await _context.GioHangChiTiets
                .Include(x => x.GiayChiTiet)
                    .ThenInclude(ct => ct.KichCo)
                .Include(x => x.GiayChiTiet)
                    .ThenInclude(ct => ct.MauSac)
                .Include(x => x.Giays)
                .FirstOrDefaultAsync(x => x.GioHangChiTietId == id);
        }



        public async Task AddAsync(GioHangChiTiet entity)
        {
            await _context.GioHangChiTiets.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(GioHangChiTiet entity)
        {
            _context.GioHangChiTiets.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _context.GioHangChiTiets.FindAsync(id);
            if (entity != null)
            {
                _context.GioHangChiTiets.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<int> GetSoLuongTonKhoAsync(Guid giayChiTietId)
        {
            return await _context.GiayChiTiets
                .Where(x => x.GiayChiTietId == giayChiTietId)
                .Select(x => x.SoLuongCon)
                .FirstOrDefaultAsync();
        }
        
        // them 
        public async Task<GiayChiTiet?> GetGiayChiTietByIdAsync(Guid giayChiTietId)
        {
            return await _context.GiayChiTiets
                .FirstOrDefaultAsync(x => x.GiayChiTietId == giayChiTietId);
        }
        public async Task<GioHangChiTiet?> GetByGioHangVaGiayChiTietAsync(
        Guid gioHangId,
        Guid giayChiTietId)
        {
            return await _context.GioHangChiTiets.FirstOrDefaultAsync(x =>
                x.GioHangId == gioHangId &&
                x.GiayChiTietId == giayChiTietId
            );
        }
    }


}
