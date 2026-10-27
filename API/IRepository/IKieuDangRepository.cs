using Data.Models;

namespace API.IRepository
{
    public interface IKieuDangRepository
    {
        Task<IEnumerable<KieuDang>> GetAllAsync();
        Task<KieuDang?> GetByIdAsync(Guid id);
        Task<IEnumerable<KieuDang>> SearchByNameAsync(string keyword);
        Task<KieuDang> AddAsync(KieuDang kieuDang);
        Task<KieuDang> UpdateAsync(KieuDang kieuDang);
        Task<bool> DeleteAsync(Guid id);
    }
}
