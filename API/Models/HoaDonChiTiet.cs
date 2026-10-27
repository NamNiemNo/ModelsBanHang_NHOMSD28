using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class HoaDonChiTiet
    {
        [Key]
        public Guid HoaDonChiTietId { get; set; } = Guid.NewGuid();

        public Guid GiayChiTietId { get; set; }

        public Guid HoaDonId { get; set; }

        [Range(1, int.MaxValue)]
        public int SoLuongSanPham { get; set; }

        public decimal Gia { get; set; }
        public decimal? DonGia { get; set; } 

        // Đánh dấu đây là hàng mua hay trả lại
        public bool TrangThai { get; set; } = false;

        // Ghi nhận thời điểm khách trả hàng (chỉ có nếu TrangThai = true)
        public DateTime? NgayTraHang { get; set; }

        // Trạng thái dòng chi tiết (nếu cần phân biệt logic nâng cao)
        [MaxLength(20)]
        public string? TrangThaiChiTiet { get; set; }

        // Ghi chú nếu có
        [MaxLength(255)]
        public string? GhiChu { get; set; }

        // Navigation properties
        public virtual HoaDon? HoaDons { get; set; }
        public virtual GiayChiTiet? GiayChiTiet { get; set; }
    }


}
