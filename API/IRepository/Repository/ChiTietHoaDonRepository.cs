using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace API.IRepository.Repository
{
    public class ChiTietHoaDonRepository : IChiTietHoaDonRepository
    {
        private readonly DbContextApp _contextApp;

        public ChiTietHoaDonRepository(DbContextApp contextApp)
        {
            _contextApp = contextApp;
        }

        public async Task<IEnumerable<HoaDonChiTiet>> GetAllHDCTAsync()
        {
            return await _contextApp.HoaDonChiTiets

              .Include(x => x.GiayChiTiet)
                  .ThenInclude(x => x.Giay)

              .Include(x => x.GiayChiTiet)
                  .ThenInclude(x => x.KichCo)

              .Include(x => x.GiayChiTiet)
                  .ThenInclude(x => x.MauSac)

              .Include(x => x.HoaDons)

              .ToListAsync();
        }

        public async Task<IEnumerable<HoaDonChiTiet>> GetByHoaDonChiTietIdAsync(Guid hdctID)
        {
            return await _contextApp.HoaDonChiTiets
             .Where(x => x.HoaDonChiTietId == hdctID)

             .Include(x => x.GiayChiTiet)
                 .ThenInclude(x => x.Giay)

             .Include(x => x.GiayChiTiet)
                 .ThenInclude(x => x.KichCo)

             .Include(x => x.GiayChiTiet)
                 .ThenInclude(x => x.MauSac)

             .Include(x => x.HoaDons)

             .ToListAsync();
        }

        public async Task<HoaDonChiTiet?> GetByIdHDCTAsync(Guid IdCTHD)
        {
            return await _contextApp.HoaDonChiTiets

             .Include(x => x.GiayChiTiet)
                 .ThenInclude(x => x.Giay)

             .Include(x => x.GiayChiTiet)
                 .ThenInclude(x => x.KichCo)

             .Include(x => x.GiayChiTiet)
                 .ThenInclude(x => x.MauSac)

             .Include(x => x.HoaDons)

             .FirstOrDefaultAsync(x => x.HoaDonChiTietId == IdCTHD);
        }

        public async Task<HoaDonChiTiet?> GetByHoaDonChiTietvaGiayAsync(Guid hoaDonId, Guid giayId)
        {
            return await _contextApp.HoaDonChiTiets
                .Include(x => x.GiayChiTiet)
                .Include(x => x.HoaDons)
                .FirstOrDefaultAsync(x => x.HoaDonId == hoaDonId && x.GiayChiTietId == giayId);
        }

        public async Task<bool> Create(HoaDonChiTiet hoaDonChiTiet)
        {
            try
            {
                await _contextApp.HoaDonChiTiets.AddAsync(hoaDonChiTiet);
                await _contextApp.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi thêm hóa đơn chi tiết: " + ex.Message);
                return false; 
            }
        }

    }
}
