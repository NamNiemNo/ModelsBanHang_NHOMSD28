using Data.Models;

namespace BlazorKhachHang.Service.IService
{
    public interface IChiTietHoaDonService
    {
        Task<List<HoaDonChiTiet>> GetAllHDCTAsync();
        Task<HoaDonChiTiet> GetByHoaDonChiTietIdAsync(Guid hdctID);
        Task<HoaDonChiTiet> GetByIdHDCTAsync(Guid IdCTHD);
        Task<HoaDonChiTiet> GetByHoaDonChiTietvaGiayAsync(Guid hdctID, Guid giayId);
    }
}
