using Data.Models;

namespace API.IRepository
{
    public interface IMauSacRepository
    {
        Task<IEnumerable<MauSac>> GetAllAsync();
        Task<MauSac> GetByIdAsync(Guid id);
        Task<MauSac> AddAsync(MauSac mauSac);
        Task<MauSac> UpdateAsync(MauSac mauSac);
        Task<bool> DeleteAsync(Guid id);
        Task<IEnumerable<MauSac>> SearchByTenAsync(string keyword);
    }
}
