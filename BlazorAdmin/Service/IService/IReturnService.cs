using API.Models;
using API.Models.DTO.TraHang;
using Data.Models;

namespace BlazorAdmin.Service.IService
{
    public interface IReturnService
    {
        Task<List<ReturnDTO>> SearchHoaDonAsync(string maHoaDon = null, string tenKhachHang = null, string sdt = null, DateTime? ngayTao = null, string trangThai = null);
        Task<ReturnDTO> GetReturnInfoAsync(Guid hoaDonId);
        Task<List<ReturnHistoryDTO>> GetReturnHistoryAsync(Guid hoaDonId);
        Task<List<ValidateReturnResultDTO>> ValidateReturnAsync(CreateReturnDTO request);
        Task<bool> ProcessReturnAsync(CreateReturnDTO request);
        Task<bool> DeleteReturnAsync(Guid hoaDonChiTietId);

        // Admin phê duyệt: Cập nhật trạng thái = 2, cộng kho, cập nhật trạng thái hóa đơn
        Task<bool> XacNhanTraHangAsync(Guid traHangId);

        // Admin từ chối: Cập nhật trạng thái = 3 và lưu lý do từ chối
        Task<bool> TuChoiTraHangAsync(Guid traHangId, string lyDo);

        // Lấy thông tin chi tiết một hóa đơn (dùng để hiển thị thông tin chung trên trang trả hàng)
        Task<HoaDon?> GetHoaDonByIdAsync(Guid hoaDonId);
        Task<List<TraHang>> GetYeuCauByHoaDonAsync(Guid hoaDonId);
    }
}
