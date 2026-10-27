using API.Models.DTO;
using Data.Models;

namespace BlazorKhachHang.Service.IService
{
    public interface ITheLoaiGiayService
    {
        Task<IEnumerable<TheLoaiGiayDTO>> GetAllAsync();
        Task<TheLoaiGiayDTO> GetByIdAsync(Guid id);
        Task<TheLoaiGiayDTO> AddAsync(TheLoaiGiayDTO dto);
        Task<TheLoaiGiayDTO> UpdateAsync(TheLoaiGiayDTO dto);
        Task<bool> DeleteAsync(Guid id);
        Task<IEnumerable<TheLoaiGiayDTO>> SearchByNameAsync(string keyword);
    }
}