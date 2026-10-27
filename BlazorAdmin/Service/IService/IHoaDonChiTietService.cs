using Data.Models;

namespace BlazorAdmin.Service.IService
{
    public interface IHoaDonChiTietService
    {
        Task<List<HoaDonChiTiet>> GetAllAsync();
        Task<HoaDonChiTiet> Create(HoaDonChiTiet hoaDonChiTiet);
    }
}
 