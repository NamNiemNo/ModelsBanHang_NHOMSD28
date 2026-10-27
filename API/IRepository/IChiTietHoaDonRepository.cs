
using Data.Models;

namespace API.IRepository
{
    public interface IChiTietHoaDonRepository
    {
        Task<IEnumerable<HoaDonChiTiet>> GetAllHDCTAsync();
        Task<IEnumerable<HoaDonChiTiet>> GetByHoaDonChiTietIdAsync(Guid hdctID);
        Task<HoaDonChiTiet> GetByIdHDCTAsync(Guid IdCTHD);
        Task<HoaDonChiTiet> GetByHoaDonChiTietvaGiayAsync(Guid hdctID, Guid giayId);
        Task<bool> Create(HoaDonChiTiet hoaDonChiTiet);
    }
}
