using System.ComponentModel.DataAnnotations;

namespace API.Models.DTO
{
    public class GiayDTO
    {
        public Guid GiayId { get; set; }
        [Required(ErrorMessage = "Tên giày không được để trống")]
        [RegularExpression(@"^[a-zA-ZÀÁÂÃÈÉÊÌÍÒÓÔÕÙÚĂĐĨŨƠƯàáâãèéêìíòóôõùúăđĩũơưẠ-ỹ\s0-9]+$", ErrorMessage = "Tên chỉ được chứa chữ cái tiếng Việt, số và khoảng trắng")]
        public string TenGiay { get; set; }
        public bool TrangThai { get; set; }
        public DateTime NgayTao { get; set; }

        public Guid? ThuongHieuId { get; set; }
        public Guid? ChatLieuId { get; set; }
        public Guid? TheLoaiGiayId { get; set; }
        public Guid? DeGiayId { get; set; }
        public Guid? KieuDangId { get; set; }
        public int STT { get; set; } // Số thứ tự hiển thị
        // 🔽 Tên hiển thị các liên kết
        public string? TenThuongHieu { get; set; }
        public string? TenChatLieu { get; set; }
        public string? TenTheLoaiGiay { get; set; }
        public string? TenDeGiay { get; set; }
        public string? TenKieuDang { get; set; }

        // 🔽 Danh sách chi tiết giày nếu cần
        public List<GiayChiTietDTO> ChiTietGiays { get; set; } = new();
    }
}
