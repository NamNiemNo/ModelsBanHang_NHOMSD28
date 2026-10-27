namespace API.Models.DTO.BanHang
{
    public class ProductOrder
    {
        public Guid GiayId { get; set; }
        public string TenGiay { get; set; } = "";
        public string? Size { get; set; }
        public string? AnhGiay { get; set; }
        public int Quantity { get; set; }
        public float Price { get; set; }
        public float OldPrice { get; set; }
        public float NewPrice => Price;
        public int Soluong { get; set; } = 1;
    }
}
