using API.Models;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace API.IRepository.Repository
{
    public class GiayYeuThichRepository : IGiayYeuThichRepository
    {
        private readonly DbContextApp _context;

        public GiayYeuThichRepository(DbContextApp context)
        {
            _context = context;
        }

        public async Task<IEnumerable<GiayYeuThich>> GetAllAsync()
            => await _context.giayYeuThiches.ToListAsync();

        public async Task<IEnumerable<GiayYeuThich>> GetByKhachHangIdAsync(Guid khachHangId)
            => await _context.giayYeuThiches
                   .Where(x => x.KhachHangId == khachHangId)
                   .ToListAsync();

        public async Task<GiayYeuThich> GetByIdAsync(Guid id)
            => await _context.giayYeuThiches.FindAsync(id);

        public async Task<GiayYeuThich> GetByGiayAndKhachHangAsync(Guid giayId, Guid khachHangId)
            => await _context.giayYeuThiches
                   .FirstOrDefaultAsync(x => x.GiayId == giayId && x.KhachHangId == khachHangId);

        public async Task AddAsync(GiayYeuThich item)
        {
            _context.giayYeuThiches.Add(item);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var item = await _context.giayYeuThiches.FindAsync(id);
            if (item != null)
            {
                _context.giayYeuThiches.Remove(item);
                await _context.SaveChangesAsync();
            }
        }
    }
}
