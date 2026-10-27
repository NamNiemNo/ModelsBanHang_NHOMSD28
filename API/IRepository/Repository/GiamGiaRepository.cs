using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace API.IRepository.Repository
{
    public class GiamGiaRepository : IGiamGiaRepository
    {
        private readonly DbContextApp _context;

        public GiamGiaRepository(DbContextApp context)
        {
            _context = context;
        }

        public async Task<IEnumerable<GiamGia>> GetAllAsync()
        {
            return await _context.GiamGias
                .Include(x => x.GiayDotGiamGias)
                .ThenInclude(y => y.GiayChiTiet) // ✅ sửa Giay → GiayChiTiet
                .ToListAsync();
        }

        public async Task<GiamGia> GetByIdAsync(Guid id)
        {
            return await _context.GiamGias
                .Include(x => x.GiayDotGiamGias)
                .ThenInclude(y => y.GiayChiTiet) // ✅ sửa Giay → GiayChiTiet
                .FirstOrDefaultAsync(x => x.GiamGiaId == id);
        }

        public async Task<GiamGia> AddAsync(GiamGia giamGia)
        {
            giamGia.GiamGiaId = Guid.NewGuid();
            _context.GiamGias.Add(giamGia);
            await _context.SaveChangesAsync();
            return giamGia;
        }

        public async Task<GiamGia> UpdateAsync(GiamGia giamGia)
        {
            var existing = await _context.GiamGias.FindAsync(giamGia.GiamGiaId);
            if (existing == null) return null;

            existing.TenGiamGia = giamGia.TenGiamGia;
            existing.SanPhamKhuyenMai = giamGia.SanPhamKhuyenMai;
            existing.PhanTramKhuyenMai = giamGia.PhanTramKhuyenMai;
            existing.NgayBatDau = giamGia.NgayBatDau;
            existing.NgayKetThuc = giamGia.NgayKetThuc;
            existing.TrangThai = giamGia.TrangThai;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var giamGia = await _context.GiamGias.FindAsync(id);
            if (giamGia == null) return false;

            var links = _context.GiayDotGiamGias.Where(x => x.GiamGiaId == id);
            _context.GiayDotGiamGias.RemoveRange(links);

            _context.GiamGias.Remove(giamGia);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddGiayToDotGiamGia(Guid giamGiaId, Guid giayChiTietId)
        {
            var giamGia = await _context.GiamGias.FindAsync(giamGiaId);
            var chiTiet = await _context.GiayChiTiets.FindAsync(giayChiTietId);
            if (giamGia == null || chiTiet == null) return false;

            var exists = await _context.GiayDotGiamGias
                .AnyAsync(x => x.GiamGiaId == giamGiaId && x.GiayChiTietId == giayChiTietId);
            if (exists) return false;

            var link = new GiayDotGiamGia
            {
                GiamGiaId = giamGiaId,
                GiayChiTietId = giayChiTietId
            };
            _context.GiayDotGiamGias.Add(link);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveGiayFromDotGiamGia(Guid giamGiaId, Guid giayChiTietId)
        {
            var link = await _context.GiayDotGiamGias
                .FirstOrDefaultAsync(x => x.GiamGiaId == giamGiaId && x.GiayChiTietId == giayChiTietId);
            if (link == null) return false;

            _context.GiayDotGiamGias.Remove(link);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
