using API.Models;
using API.Models.DTO.TraHang;
using Data.Models;

namespace API.IRepository
{
    public interface IReturnRepository
    {
        Task<List<HoaDon>> GetHoaDonWithChiTietAsync(string maHoaDon = null, string tenKhachHang = null, string sdt = null, DateTime? ngayTao = null, string trangThai = null);
        Task<HoaDon?> GetHoaDonByIdAsync(Guid hoaDonId);
        Task<List<HoaDonChiTiet>> GetLichSuTraHangAsync(Guid hoaDonId);
        Task<Dictionary<Guid, int>> GetSoLuongConLaiChoTraAsync(Guid hoaDonId);
        Task<bool> KiemTraConHanTraHangAsync(Guid hoaDonId);
        Task<bool> SanPhamNamTrongHoaDon(Guid hoaDonId, Guid giayId);
        Task<bool> KiemTraDuocPhepTra(Guid hoaDonId, Guid giayId, int soLuongMuonTra);
        Task<bool> TraHangAsync(Guid hoaDonId, List<ChiTietTraHangDTO> items, Guid taiKhoanId);
        Task CapNhatTrangThaiHoaDonNeuTraHet(Guid hoaDonId);
        Task<List<HoaDonChiTiet>> GetDanhSachSanPhamTrongHoaDon(Guid hoaDonId);
        Task<bool> HuyTraHangAsync(Guid hoaDonChiTietId);
        Task<bool> CapNhatGhiChuTraHang(Guid hoaDonChiTietId, string ghiChuMoi);
        Task<string> GetTenGiayAsync(Guid giayId); // Thêm phương thức mới
                                                   // Lấy danh sách các yêu cầu trả hàng (TraHang) của một hóa đơn cụ thể
        Task<List<TraHang>> GetYeuCauTraHangTheoHoaDonAsync(Guid hoaDonId);

        // Admin phê duyệt: Cập nhật trạng thái = 2, cộng kho, cập nhật trạng thái hóa đơn
        Task<bool> XacNhanTraHangAsync(Guid traHangId);

        // Admin từ chối: Cập nhật trạng thái = 3 và lưu lý do từ chối
        Task<bool> TuChoiTraHangAsync(Guid traHangId, string lyDo);

        // Lấy thông tin chi tiết một hóa đơn (dùng để hiển thị thông tin chung trên trang trả hàng)


    }
}
