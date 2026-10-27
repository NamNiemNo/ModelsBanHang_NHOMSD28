using API.Models.DTO;
using Data.Models;

namespace API.IRepository
{
    public interface IGioHangRepository
    {
        Task<IEnumerable<GioHang>> GetAllGioHang();
        Task<GioHang> GetGioHang(Guid id);
        Task CreateGioHang(GioHang gh);
        Task UpdateGioHang(GioHang gh);
        Task DeleteGioHang(Guid id);
        Task<GioHang?> GetGioHangByKhachHang(Guid khachHangId); // thêm 
        Task CapNhatSoLuong(Guid gioHangChiTietId, int soLuongMoi);


    }
}
