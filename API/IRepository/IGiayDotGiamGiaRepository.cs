using Data.Models;

namespace API.IRepository
{
    public interface IGiayDotGiamGiaRepository
    {
        Task<IEnumerable<GiayDotGiamGia>> GetAllAsync();
        Task<IEnumerable<GiayDotGiamGia>> GetByGiamGiaIdAsync(Guid giamGiaId);
        Task<GiayDotGiamGia?> GetByIdAsync(Guid id);
        Task<GiayDotGiamGia> AddAsync(GiayDotGiamGia entity);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid giamGiaId, Guid giayChiTietId);
    }
}
