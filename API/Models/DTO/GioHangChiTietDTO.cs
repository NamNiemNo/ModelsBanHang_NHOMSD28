namespace API.Models.DTO
{
    public class GioHangChiTietDTO
    {
        public Guid GioHangChiTietId { get; set; }
        public Guid GiayChiTietId { get; set; }
        public string TenGiay { get; set; }
        public string? Size { get; set; }
        public string? MauSac { get; set; }
        public string? HinhAnh { get; set; }
        public int SoLuong { get; set; }
        public decimal Gia { get; set; }
        
        public decimal ThanhTien => Gia * SoLuong;
        public bool IsSelected { get; set; } = true;
    }
}
