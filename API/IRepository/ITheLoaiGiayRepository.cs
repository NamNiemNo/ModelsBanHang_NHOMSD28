using Data.Models;

namespace Data.IRepository
{
    public interface ITheLoaiGiayRepository
    {
        Task<IEnumerable<TheLoaiGiay>> SearchByNameAsync(string keyword);

        Task<IEnumerable<TheLoaiGiay>> GetAllAsync();
        Task<TheLoaiGiay> GetByIdAsync(Guid id);
        Task<TheLoaiGiay> AddAsync(TheLoaiGiay theLoaiGiay);
        Task<TheLoaiGiay> UpdateAsync(TheLoaiGiay theLoaiGiay);
        Task<bool> DeleteAsync(Guid id);

        // (Tuỳ chọn) Lấy thể loại của một giày chi tiết
        Task<TheLoaiGiay> GetTheLoaiByGiayChiTietIdAsync(Guid giayChiTietId);
    }
}
