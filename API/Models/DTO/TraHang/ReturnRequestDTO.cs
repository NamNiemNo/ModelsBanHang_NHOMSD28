namespace API.Models.DTO.TraHang
{
    public class ReturnRequestDTO
    {
        public Guid TraHangId { get; set; }
        public Guid HoaDonId { get; set; }
        public Guid GiayChiTietId { get; set; }
        public string TenSanPham { get; set; }
        public int SoLuong { get; set; }
        public decimal GiaTaiThoiDiemTra { get; set; }
        public DateTime NgayYeuCau { get; set; }

        public int TrangThai { get; set; }

        public string? GhiChu { get; set; }
        public string? LyDoTuChoi { get; set; }
    }
}