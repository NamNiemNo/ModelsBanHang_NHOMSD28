using System.Text.Json.Serialization;

namespace API.Models.DTO.BanHang
{
    // thêm
    public class HoaDonKhachHangDTO
    {

        public Guid HoaDonId { get; set; }

        public string TrangThai { get; set; } = "";
        public DateTime NgayTao { get; set; }

        public string TenNguoiNhan { get; set; } = "";
        public string SoDienThoai { get; set; } = "";
        public string DiaChi { get; set; } = "";
        public decimal PhiShip { get; set; }
        public decimal GiamGiaVoucher { get; set; }
        public decimal TongTien { get; set; }

        public string HinhThucThanhToan { get; set; } = "";
        public string TrangThaiThanhToan { get; set; } = "";

        public List<HoaDonChiTietDTO> ChiTiet { get; set; } = new();
    
    }
}
