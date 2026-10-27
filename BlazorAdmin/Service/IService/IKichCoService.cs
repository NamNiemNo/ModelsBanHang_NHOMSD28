using API.Models.DTO;
using Data.Models;

namespace API.IService
{
    public interface IKichCoService
    {
        Task<List<KichCoDTO>> GetAllAsync();
        Task<KichCoDTO> GetByIdAsync(Guid id);
        Task CreateAsync(KichCoDTO obj);
        Task UpdateAsync(KichCoDTO obj);
        Task DeleteAsync(Guid id);
        Task<List<KichCoDTO>> SearchAsync(string keyword);
    }
}