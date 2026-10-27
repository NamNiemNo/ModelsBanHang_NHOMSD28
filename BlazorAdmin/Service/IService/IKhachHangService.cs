using Data.Models;
namespace BlazorAdmin.Service.IService
{
    public interface IKhachHangService
    {
        Task<List<KhachHang>> GetAll();
        Task<KhachHang?> GetById(Guid id);
        Task<KhachHang> Create(KhachHang khachHang);
        Task Update(KhachHang khachHang);
        Task Delete(Guid id);
        Task<List<KhachHang>> SearchKhachHangAsync(string keyword);

    }
}
