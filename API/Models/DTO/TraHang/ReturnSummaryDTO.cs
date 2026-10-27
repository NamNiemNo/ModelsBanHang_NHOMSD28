namespace API.Models.DTO.TraHang
{
    public class ReturnSummaryDTO
    {
        public Guid HoaDonId { get; set; }
        public string TenCuaKhachHang { get; set; }
        public string SDTCuaKhachHang { get; set; }
        public DateTime NgayTao { get; set; }
        public decimal TongTienSauKhiGiam { get; set; }
        public string TrangThai { get; set; }
    }

}