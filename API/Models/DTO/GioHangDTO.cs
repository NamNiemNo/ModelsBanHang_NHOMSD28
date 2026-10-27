namespace API.Models.DTO
{
    public class GioHangDTO
    {
        public List<GioHangChiTietDTO> DanhSachChiTiet { get; set; } = new();
        public decimal TongTien => DanhSachChiTiet.Sum(x => x.ThanhTien);
    }
}
