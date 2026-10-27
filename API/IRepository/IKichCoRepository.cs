using Data.Models;
using System.Security.Cryptography;

namespace API.IRepository
{
    public interface IKichCoRepository
    {
        Task<IEnumerable<KichCo>> GetAllAsync();
        Task<KichCo> GetByIdAsync(Guid id);
        Task<KichCo> AddAsync(KichCo kichCo);
        Task<KichCo> UpdateAsync(KichCo kichCo);
        Task<bool> DeleteAsync(Guid id);
        Task<IEnumerable<KichCo>> SearchByTenAsync(string keyword);
    }
}
