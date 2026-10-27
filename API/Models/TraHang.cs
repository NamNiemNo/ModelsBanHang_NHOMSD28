using Data.Models;

namespace API.Models
{
    public class TraHang
    {
        public Guid TaiKhoanId { get; set; }
        public Guid Id { get; set; }
        public Guid HoaDonId { get; set; }
        public Guid GiayChiTietId { get; set; }
        public int SoLuong { get; set; }
        public decimal GiaTaiThoiDiemTra { get; set; }
        public DateTime NgayYeuCau { get; set; }
        public string? GhiChu { get; set; }
        public string? LyDoTuChoi { get; set; }

        // 1: Chờ xác nhận, 2: Đã nhận hàng/Hoàn tiền, 3: Từ chối
        public int TrangThai { get; set; }

        // Navigation properties
        public virtual HoaDon HoaDon { get; set; }
        public virtual GiayChiTiet GiayChiTiet { get; set; }
    }
}