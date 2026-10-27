using Application.DTOs;
using Data.Models;
namespace BlazorKhachHang.Service.IService
{
    public interface IKhachHangService
    {
        Task<List<KhachHang>> GetAll();
        Task<KhachHang?> GetById(Guid id);
        Task Create(KhachHang khachHang);
        Task Update(KhachHang khachHang);
        Task Delete(Guid id);
        Task<List<KhachHang>> SearchKhachHangAsync(string keyword);
        // thêm địa chỉ
        Task AddAddress(DiaChiKhachHangDto dto);

        // lấy địa chỉ theo khách hàng
        Task<List<DiaChiKhachHangDto>> GetAddress(Guid khachHangId);

    }
}
