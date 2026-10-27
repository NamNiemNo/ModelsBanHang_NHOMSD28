using API.Models.DTO;

namespace BlazorKhachHang.Service.IService
{
    public interface IMauSacService
    {
        Task<List<MauSacDTO>> GetAllAsync();
        Task<MauSacDTO> GetByIdAsync(Guid id);
        Task CreateAsync(MauSacDTO obj);
        Task UpdateAsync(MauSacDTO obj);
        Task DeleteAsync(Guid id);
        Task<List<MauSacDTO>> SearchAsync(string keyword);
    }
}
