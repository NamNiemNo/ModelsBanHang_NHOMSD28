using API.Models.DTO.BanHang;
using Data.Models;
using static API.Models.DTO.BanHang.DatHang;

namespace BlazorKhachHang.Service.IService
{
    public interface IHoaDonService
    {
        Task Add(HoaDon hoaDon);

        Task<List<HoaDon>> GetAll();

        Task<HoaDon?> GetById(Guid id);

        Task Update(HoaDon hoaDon);

        Task Delete(Guid id);

        // Đặt hàng COD.
        Task<bool> DatHangAsync(
            DatHangRequest request);

        // Lấy phí ship từ API.
        Task<TinhPhiShipResponse> TinhPhiShipAsync(
            TinhPhiShipRequest request);

        // Lấy đơn hàng của khách hàng.
        Task<List<HoaDonKhachHangDTO>>
            GetDonHangKhachHangAsync(
                Guid khachHangId);

        // Lấy đơn hàng theo trạng thái.
        Task<List<HoaDonKhachHangDTO>>
            GetDonHangKhachHangAsync(
                Guid khachHangId,
                string trangThai);

        // Cập nhật thông tin giao hàng.
        Task CapNhatThongTinGiaoHangAsync(
            UpdateThongTinGiaoHangDTO dto);

        // Khách hàng hủy đơn.
        Task HuyDonHangAsync(
            Guid hoaDonId,
            string lyDo);

        // Lấy chi tiết hóa đơn.
        Task<HoaDonKhachHangDTO?>
            GetHoaDonByIdAsync(
                Guid hoaDonId);
    }
}