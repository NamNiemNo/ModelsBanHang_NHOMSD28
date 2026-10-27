namespace API.Models.DTO.BanHang
{
    public class ThongTinHoaDon
    {

        public string TenNguoiNhan { get; set; }
        public string Sdt { get; set; }
        public string TinhThanhPho { get; set; }
        public string QuanHuyen { get; set; }
        public string XaPhuongThiTran { get; set; }
        public string DiaChiCuThe { get; set; }
        public string? GhiChu { get; set; }
        public bool IsDelivery { get; set; } = false;
        public string TrangThai { get; set; } = "Chờ xác nhận"; // thêm

    }
}
