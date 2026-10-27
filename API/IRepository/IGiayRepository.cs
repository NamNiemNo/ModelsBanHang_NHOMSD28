using Data.Models;

namespace API.IRepository
{
    public interface IGiayRepository
    {
        Task<IEnumerable<Giay>> GetAllAsync();
        Task<Giay> GetByIdAsync(Guid id);
        Task<Giay> AddAsync(Giay giay);
        Task<Giay> UpdateAsync(Giay giay);
        Task<bool> DeleteAsync(Guid id);
        Task<IEnumerable<Giay>> SearchByTenAsync(string keyword);
        Task<bool> AddWithChiTietAsync(Giay giay, List<GiayChiTiet> chiTietList);


    }
}
