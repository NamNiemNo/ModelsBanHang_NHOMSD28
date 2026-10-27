using System.ComponentModel.DataAnnotations;
using Data.Models;

namespace API.Models.BoxChatAI
{
    public class CuocTroChuyenAI
    {
        [Key]
        public Guid CuocTroChuyenAIId { get; set; }
            = Guid.NewGuid();

        // Có giá trị khi khách đã đăng nhập
        public Guid? KhachHangId { get; set; }

        // Nhận diện khách chưa đăng nhập
        public Guid PhienKhachId { get; set; }

        [Required]
        [MaxLength(150)]
        public string TieuDe { get; set; }
            = "Cuộc trò chuyện mới";

        public DateTime NgayTao { get; set; }
            = DateTime.UtcNow;

        public DateTime NgayCapNhat { get; set; }
            = DateTime.UtcNow;

        public bool DaXoa { get; set; }

        public virtual KhachHang? KhachHang { get; set; }

        public virtual ICollection<TinNhanAI> TinNhans { get; set; }
            = new List<TinNhanAI>();
    }
}