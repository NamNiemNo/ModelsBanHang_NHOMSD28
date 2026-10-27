using Data.Models;

namespace API.IRepository
{
    public interface INhanVienRepository
    {
        Task<List<NhanVien>> GetAllNhanVienAsync();
        Task<NhanVien> GetByIdNhanVienAsync(Guid id);
        Task CreateNhanVien(NhanVien nhanVien);
        Task DeleteNhanVienAsync(Guid id);
        Task UpdateNhanVienAsync(NhanVien nhanVien);

        Task<NhanVien> GetIdNhanVienTaiKhoan(Guid TK);
        Task<List<NhanVien>> SearchNhanVienAsync(string keyword);
    }
}
