using Data.Models;

namespace BlazorKhachHang.Service.IService
{
    public interface IHoaDonChiTietService
    {
        Task<List<HoaDonChiTiet>> GetAllAsync();
    }
}
