using System.ComponentModel.DataAnnotations;

namespace API.Models.DTO
{
    public class GiamGiaCreateDTO
    {
        public Guid GiamGiaId { get; set; } = Guid.NewGuid();
        public string TenGiamGia { get; set; }
        public string? SanPhamKhuyenMai { get; set; }
        public float PhanTramKhuyenMai { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
        public bool TrangThai { get; set; }

        public List<Guid> GiayChiTietIds { get; set; } = new(); // ✅ Đổi tên
    }
}
