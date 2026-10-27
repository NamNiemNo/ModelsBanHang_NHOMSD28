namespace API.Models.DTO.TraHang
{
    public class ReturnHistoryDTO
    {
        public Guid HoaDonChiTietId { get; set; }
        public Guid GiayId { get; set; }
        public string TenSanPham { get; set; }
        public int SoLuong { get; set; }
        public decimal Gia { get; set; }
        public DateTime? NgayTraHang { get; set; }
        public string? GhiChu { get; set; }
    }
}