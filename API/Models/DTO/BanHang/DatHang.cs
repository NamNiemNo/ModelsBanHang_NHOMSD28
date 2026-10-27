namespace API.Models.DTO.BanHang
{
    public class DatHang
    {
        public class DatHangRequest
        {
            public Guid HoaDonId { get; set; }
            public Guid KhachHangId { get; set; }

            // API sẽ tự tính lại hai giá trị này khi tạo đơn.
            public float PhiShip { get; set; }
            public float TongTienSauKhiGiam { get; set; }

            public string? GhiChu { get; set; }
            public string TenNguoiNhan { get; set; } = "";
            public string SoDienThoai { get; set; } = "";
            public string DiaChi { get; set; } = "";

            // Dùng để API xác định khu vực giao hàng.
            public string TinhThanhPho { get; set; } = "";

            public Guid HinhThucThanhToanId { get; set; }
            public Guid? VoucherId { get; set; }

            public List<DatHangItem> Items { get; set; } = new();

            public string PaymentMethod { get; set; } = "";
        }

        public class DatHangItem
        {
            public Guid GioHangChiTietId { get; set; }
            public Guid GiayChiTietId { get; set; }
            public int SoLuong { get; set; }
            public decimal Gia { get; set; }
        }

        public class TinhPhiShipRequest
        {
            public string TinhThanhPho { get; set; } = "";
            public decimal TamTinh { get; set; }
        }

        public class TinhPhiShipResponse
        {
            public decimal PhiShip { get; set; }
            public decimal MienPhiTu { get; set; }
            public decimal ConThieuDeMienPhi { get; set; }
            public int SoNgayGiaoDuKien { get; set; }
            public DateTime NgayGiaoDuKien { get; set; }
        }
    }
}