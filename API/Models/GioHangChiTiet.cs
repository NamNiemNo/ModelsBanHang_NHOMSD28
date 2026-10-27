using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models
{
    public class GioHangChiTiet
    {
        [Key]
        public Guid GioHangChiTietId { get; set; }

        // thêm
        public Guid GiayChiTietId { get; set; }
        public Guid GioHangId { get; set; }

		public Guid GiayId { get; set; }
        public int SoLuongSanPham { get; set; }

      
        public decimal Gia { get; set; }

        public decimal ThanhTien => Gia * SoLuongSanPham;

        public DateTime NgayTao { get; set; } = DateTime.Now;

        public DateTime NgayCapNhat { get; set; } = DateTime.Now;

        public bool TrangThai { get; set; }
        public virtual GioHang? GioHang { get; set; }
        public virtual Giay? Giays { get; set; }
        public virtual GiayChiTiet? GiayChiTiet { get; set; }
    }
}
