using API.Models.DTO;


namespace BlazorKhachHang.Service.IService
{
    public interface IDeGiayService
    {
        Task<List<DeGiayDTO>> GetAllAsync();
        Task<DeGiayDTO> GetByIdAsync(Guid id);
        Task<bool> CreateAsync(DeGiayDTO obj);
        Task<bool> UpdateAsync(DeGiayDTO obj);
        Task<bool> DeleteAsync(Guid id);
    }

}


