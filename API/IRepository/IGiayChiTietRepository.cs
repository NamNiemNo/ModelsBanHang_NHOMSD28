using Data.Models;

namespace API.IRepository
{
    public interface IGiayChiTietRepository
    {
        // Lấy toàn bộ danh sách
        Task<IEnumerable<GiayChiTiet>> GetAllAsync();

        // Lấy theo ID (có include các quan hệ)
        Task<GiayChiTiet?> GetByIdAsync(Guid id);

        // Lấy danh sách theo GiayId
        Task<IEnumerable<GiayChiTiet>> GetByGiayIdAsync(Guid giayId);

        // Thêm mới 1 bản ghi
        Task<GiayChiTiet> AddAsync(GiayChiTiet chiTiet);

        // Thêm nhiều bản ghi (trả về list đã thêm)
        Task<List<GiayChiTiet>> AddMultipleReturnAsync(List<GiayChiTiet> list);

        // Cập nhật bản ghi
        Task<GiayChiTiet?> UpdateAsync(GiayChiTiet chiTiet);
    }
}
