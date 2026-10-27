namespace API.Models.DTO.BanHang
{
    public class KhachHangUpdateDTO
    {
        public Guid KhachHangId { get; set; }
        public string HoTen { get; set; }
        public string Email { get; set; }
        public string SoDienThoai { get; set; }
        public string GioiTinh { get; set; }
        public DateTime? NgaySinh { get; set; }
    }
}
