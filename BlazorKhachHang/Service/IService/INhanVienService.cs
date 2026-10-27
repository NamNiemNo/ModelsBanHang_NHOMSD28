using Data.Models;
namespace BlazorKhachHang.Service.IService
{
    public interface INhanVienService
    {
        Task<List<NhanVien>> GetAllNhanVienAsync();
        Task<NhanVien> GetByIdNhanVienAsync(Guid NhanVienId);
        Task CreateNhanVien(NhanVien nhanVien);
        Task DeleteNhanVienAsync(Guid NhanVienId);
        Task UpdateNhanVienAsync(NhanVien nhanVien);
        Task<List<NhanVien>> SearchNhanVien(string keyword); 
    }
}