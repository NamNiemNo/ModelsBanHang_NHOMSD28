using API.Models.DTO;
using Data.Models;
using Microsoft.AspNetCore.Components.Forms;

namespace BlazorKhachHang.Service.IService
{
    public interface IAnhService
    {
        Task<List<AnhDTO>> GetAllAsync();
        Task<AnhDTO> GetByIdAsync(Guid id);

        Task<AnhDTO> UploadAsync(IBrowserFile file, string tenAnh, Guid giayChiTietId);
        Task<AnhDTO> UpdateAsync(AnhDTO dto);

        Task<AnhDTO> UpdateFileAsync(Guid id, IBrowserFile file, string tenAnh, Guid giayChiTietId);
        Task<bool> DeleteAsync(Guid id);
    }
}