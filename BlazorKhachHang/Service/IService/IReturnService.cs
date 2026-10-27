using API.Models;
using API.Models.DTO.TraHang;
using Data.Models;

namespace BlazorKhachHang.Service.IService
{
    public interface IReturnService
    {
        Task<List<ReturnDTO>> SearchHoaDonAsync(string maHoaDon = null, string tenKhachHang = null, string sdt = null, DateTime? ngayTao = null, string trangThai = null);
        Task<ReturnDTO> GetReturnInfoAsync(Guid hoaDonId);
        Task<List<ReturnHistoryDTO>> GetReturnHistoryAsync(Guid hoaDonId);
        Task<List<ValidateReturnResultDTO>> ValidateReturnAsync(CreateReturnDTO request);
        Task<bool> ProcessReturnAsync(CreateReturnDTO request);
        Task<bool> DeleteReturnAsync(Guid hoaDonChiTietId);
        Task<List<TraHang>> GetYeuCauByHoaDonAsync(Guid hoaDonId);
        Task<List<ReturnHistoryDTO>> GetReturnHistory(Guid hoaDonId);

    }
}
