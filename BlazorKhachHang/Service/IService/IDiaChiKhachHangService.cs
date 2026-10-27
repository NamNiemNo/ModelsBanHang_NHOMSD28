using API.Models.DTO;
using Application.DTOs;

namespace BlazorKhachHang.Service.IService
{
    public interface IDiaChiKhachHangService
    {
        Task<List<DiaChiKhachHangDto>> GetAllAsync();
        Task<DiaChiKhachHangDto> GetByIdAsync(Guid id);
        Task<List<DiaChiKhachHangDto>> GetByKhachHangIdAsync(Guid khachHangId);
        Task<DiaChiKhachHangDto> GetDefaultByKhachHangIdAsync(Guid khachHangId);
        Task<bool> CreateAsync(DiaChiKhachHangDto dto);
        Task<bool> UpdateAsync(Guid id, DiaChiKhachHangDto dto);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> SetDefaultAsync(Guid id);
    }
}
