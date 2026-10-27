namespace API.Models.DTO.BanHang
{
    public class UpdateThongTinGiaoHangDTO
    {
        public Guid HoaDonId { get; set; }

        public string? TenNguoiNhan { get; set; }
        public string? SoDienThoai { get; set; }

   
        public string? DiaChi { get; set; }


        public string? DiaChiCuThe { get; set; }
        public string? PhuongXa { get; set; }
        public string? QuanHuyen { get; set; }
        public string? ThanhPho { get; set; }
    }
}
