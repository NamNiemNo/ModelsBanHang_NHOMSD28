using static BlazorKhachHang.Components.Pages.Oder;

namespace BlazorKhachHang.ViewModel
{
    public class OrderModel
    {
        public Guid HoaDonId { get; set; }
        public string ShopName { get; set; } = "Shop giày";
        public string TrangThai { get; set; } = "";
        public string Category { get; set; }
        public decimal TongTien { get; set; }
        public DateTime ReviewDeadline { get; set; }
        public string HinhThucThanhToan { get; set; } = "";
        public string TrangThaiThanhToan { get; set; } = "";
        public List<OrderItemModel> Items { get; set; }
    }
}
