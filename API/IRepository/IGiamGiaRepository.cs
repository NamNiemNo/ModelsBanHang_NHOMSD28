using Data.Models;

namespace API.IRepository
{
    public interface IGiamGiaRepository
    {
        Task<IEnumerable<GiamGia>> GetAllAsync();
        Task<GiamGia> GetByIdAsync(Guid id);
        Task<GiamGia> AddAsync(GiamGia giamGia);
        Task<GiamGia> UpdateAsync(GiamGia giamGia);
        Task<bool> DeleteAsync(Guid id);

        // Thêm/Xóa giày vào/ra đợt giảm giá
        Task<bool> AddGiayToDotGiamGia(Guid giamGiaId, Guid giayChiTietId);
        Task<bool> RemoveGiayFromDotGiamGia(Guid giamGiaId, Guid giayChiTietId);

    }
}
