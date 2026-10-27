namespace API.Models.DTO.BanHang
{
    // thêm 
    public class HoaDonChiTietDTO
    {
        public Guid GiayChiTietId { get; set; }
        public int SoLuongSanPham { get; set; }
        public decimal Gia { get; set; }
        public string? MauSac {  get; set; }
        public string TenGiay { get; set; }
        public string? Anh { get; set; }
        public string? Size { get; set; }
        public decimal DonGia { get; set; }
    }
}
