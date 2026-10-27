using API.Models.DTO;
namespace BlazorKhachHang.Service.IService
{
    public interface IGiayService
    {
        Task<List<GiayDTO>> GetAllAsync();
        Task<GiayDTO> GetByIdAsync(Guid id);
        Task<GiayDTO> CreateAsync(GiayDTO obj);

        Task UpdateAsync(GiayDTO obj);
        Task DeleteAsync(Guid id);
        Task<List<GiayDTO>> SearchAsync(string keyword);
    }
}
