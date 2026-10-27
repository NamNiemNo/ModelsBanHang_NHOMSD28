using API.Models.DTO;
using Data.Models;

namespace API.IRepository
{
    public interface IGioHangChiTietRepository
    {
        Task<IEnumerable<GioHangChiTiet>> GetAllAsync();
        Task<IEnumerable<GioHangChiTiet>> GetByGioHangIdAsync(Guid gioHangId); // Lấy chi tiết giỏ hàng theo giỏ
        Task<GioHangChiTiet> GetByIdAsync(Guid id);
        Task<GioHangChiTiet?> GetByGioHangVaGiayChiTietAsync(
         Guid gioHangId,
         Guid giayChiTietId
     ); // Kiểm tra sản phẩm trong giỏ  
        Task<int> GetSoLuongTonKhoAsync(Guid giayChiTietId); // Kiểm tra số lượng sản phẩm
      
        Task AddAsync(GioHangChiTiet entity);
        Task UpdateAsync(GioHangChiTiet entity);
        Task DeleteAsync(Guid id);
        Task<GiayChiTiet?> GetGiayChiTietByIdAsync(Guid giayChiTietId);
    }

}
