using API.Models.DTO;

namespace BlazorKhachHang.Service.IService
{
    public interface IKieuDangService
    {
        Task<IEnumerable<KieuDangDTO>> GetAllAsync();
        Task<KieuDangDTO?> GetByIdAsync(Guid id);
        Task<IEnumerable<KieuDangDTO>> SearchByNameAsync(string keyword);
        Task<KieuDangDTO> CreateAsync(KieuDangDTO dto);
        Task<KieuDangDTO?> UpdateAsync(Guid id, KieuDangDTO dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
