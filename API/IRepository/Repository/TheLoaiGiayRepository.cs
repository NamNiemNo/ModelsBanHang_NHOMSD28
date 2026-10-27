using Data.IRepository;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Data.Repository
{
    public class TheLoaiGiayRepository : ITheLoaiGiayRepository
    {
        private readonly DbContextApp _context;

        public TheLoaiGiayRepository(DbContextApp context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TheLoaiGiay>> GetAllAsync()
        {
            return await _context.TheLoaiGiays.ToListAsync();
        }

        public async Task<TheLoaiGiay> GetByIdAsync(Guid id)
        {
            return await _context.TheLoaiGiays
                .Include(t => t.Giays)
                .FirstOrDefaultAsync(t => t.TheLoaiGiayId == id);
        }

        public async Task<TheLoaiGiay> AddAsync(TheLoaiGiay theLoaiGiay)
        {
            _context.TheLoaiGiays.Add(theLoaiGiay);
            await _context.SaveChangesAsync();
            return theLoaiGiay;
        }

        public async Task<TheLoaiGiay> UpdateAsync(TheLoaiGiay theLoaiGiay)
        {
            var existing = await _context.TheLoaiGiays.FindAsync(theLoaiGiay.TheLoaiGiayId);
            if (existing == null) return null;

            existing.TenTheLoai = theLoaiGiay.TenTheLoai;
            existing.MoTa = theLoaiGiay.MoTa;
            existing.TrangThai = theLoaiGiay.TrangThai;
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _context.TheLoaiGiays.FindAsync(id);
            if (entity == null) return false;

            _context.TheLoaiGiays.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        // (Tuỳ chọn) Lấy thể loại của một giày chi tiết
        public async Task<TheLoaiGiay> GetTheLoaiByGiayChiTietIdAsync(Guid giayId)
        {
            var giay = await _context.Giays
                .Include(g => g.TheLoaiGiay)
                .FirstOrDefaultAsync(g => g.GiayId == giayId);

            return giay?.TheLoaiGiay;
        }
        public async Task<IEnumerable<TheLoaiGiay>> SearchByNameAsync(string keyword)
        {
            return await _context.TheLoaiGiays
                .Where(t => t.TenTheLoai.Contains(keyword))
                .ToListAsync();
        }

    }
}