namespace BlazorKhachHang.ViewModel
{
    public class OrderItemModel
    {
        public Guid GiayChiTietId { get; set; }
        public string ProductName { get; set; }
        public string MauSac { get; set; }
        public string Size { get; set; }
        public int Quantity { get; set; }
        // giá
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice => UnitPrice * Quantity;
        public string ImageUrl { get; set; }
    }
}
