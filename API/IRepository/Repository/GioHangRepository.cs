using API.Models.DTO;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace API.IRepository.Repository
{
    public class GioHangRepository : IGioHangRepository
    {
        private readonly DbContextApp _dbcontext;

        public GioHangRepository(DbContextApp dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task CreateGioHang(GioHang gh)
        {
            try
            {
                _dbcontext.Add<GioHang>(gh);
                await _dbcontext.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task DeleteGioHang(Guid id)
        {
            try
            {
                var ghdel = await _dbcontext.GioHangs.FindAsync(id);
                if (ghdel != null) { 
                    _dbcontext.Remove<GioHang>(ghdel);
                    await _dbcontext.SaveChangesAsync();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IEnumerable<GioHang>> GetAllGioHang()
        {
            return await _dbcontext.GioHangs.ToListAsync();
        }

        public async Task<GioHang> GetGioHang(Guid id)
        {
            return await _dbcontext.GioHangs.FindAsync(id);
        }

        public async Task UpdateGioHang(GioHang gh)
        {
            try
            {
                _dbcontext.Update<GioHang>(gh);
                await _dbcontext.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }
        // thêm 
        public async Task<GioHang?> GetGioHangByKhachHang(Guid khachHangId)
        {
            return await _dbcontext.GioHangs
                .Where(g => g.KhachHangId == khachHangId)

                .Include(g => g.GioHangChiTiets)
                    .ThenInclude(ct => ct.GiayChiTiet)
                        .ThenInclude(gct => gct.Giay)

                .Include(g => g.GioHangChiTiets)
                    .ThenInclude(ct => ct.GiayChiTiet)
                        .ThenInclude(gct => gct.KichCo)

                .Include(g => g.GioHangChiTiets)
                    .ThenInclude(ct => ct.GiayChiTiet)
                        .ThenInclude(gct => gct.MauSac)

                .Include(g => g.GioHangChiTiets)
                    .ThenInclude(ct => ct.GiayChiTiet)
                        .ThenInclude(gct => gct.Anhs)

                .FirstOrDefaultAsync();
        }
        public async Task CapNhatSoLuong(
        Guid gioHangChiTietId,
        int soLuongMoi)
        {
            var item = await _dbcontext.GioHangChiTiets
                .Include(x => x.GiayChiTiet)
                .FirstOrDefaultAsync(
                    x => x.GioHangChiTietId == gioHangChiTietId);

            if (item == null)
                throw new Exception("Không tìm thấy sản phẩm");

            // số lượng tồn kho mới nhất
            int tonKho = item.GiayChiTiet.SoLuongCon;

            // đã hết hàng
            if (tonKho <= 0)
            {
                _dbcontext.GioHangChiTiets.Remove(item);
                await _dbcontext.SaveChangesAsync();

                throw new Exception(
                    "Sản phẩm đã hết hàng và được xóa khỏi giỏ");
            }

            // vượt quá tồn kho
            if (soLuongMoi > tonKho)
            {
                item.SoLuongSanPham = tonKho;

                await _dbcontext.SaveChangesAsync();

                throw new Exception(
                    $"Chỉ còn {tonKho} sản phẩm");
            }

            item.SoLuongSanPham = soLuongMoi;

            await _dbcontext.SaveChangesAsync();
        }
    }
}
