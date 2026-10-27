using Data.Models;

namespace API.Models.DTO.BanHang
{
    public class Order
    {
        public List<GiayChiTietDTO> Products { get; set; }
            = new();

        public KhachHang? Customer { get; set; }

        public DiaChiKhachHang? Address { get; set; }

        public ThongTinHoaDon ThongTinHoaDon { get; set; }
            = new();

        // Lưu phí ship được trả về từ API
        // cho từng tab đơn hàng.
        public float PhiShip { get; set; }

        public DateTime ThoiGianTao { get; set; }
            = DateTime.Now;
    }
}