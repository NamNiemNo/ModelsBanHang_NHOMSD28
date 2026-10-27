using API.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class Giay
    {
        [Key]
        public Guid GiayId { get; set; }
        public Guid? ChatLieuId { get; set; }
        public Guid? ThuongHieuId { get; set; }
        public Guid? KieuDangId { get; set; }
        public Guid? TheLoaiGiayId { get; set; }
        public Guid? DeGiayId { get; set; }
        [Required]
        [StringLength(50)]
        [RegularExpression(@"^[a-zA-ZÀÁÂÃÈÉÊÌÍÒÓÔÕÙÚĂĐĨŨƠƯàáâãèéêìíòóôõùúăđĩũơưẠ-ỹ\s0-9]+$", ErrorMessage = "Tên chỉ được chứa chữ cái tiếng Việt, số và khoảng trắng")]
        public string TenGiay { get; set; }
        public DateTime NgayTao { get; set; }
        public bool TrangThai { get; set; }
        public virtual ThuongHieu? ThuongHieu { get; set; }
        public virtual KieuDang? KieuDang { get; set; }
        public virtual DeGiay? DeGiay { get; set; }
        public virtual TheLoaiGiay? TheLoaiGiay { get; set; }
        public virtual ChatLieu? ChatLieu { get; set; }
        public virtual ICollection<HoaDonChiTiet>? HoaDonChiTiets { get; set; } = new List<HoaDonChiTiet>();
        public virtual ICollection<GiayDotGiamGia> GiayDotGiamGias { get; set; } = new List<GiayDotGiamGia>();
        public virtual ICollection<GiayChiTiet> GiayChiTiets { get; set; } = new List<GiayChiTiet>();
        public virtual ICollection<GioHangChiTiet>? GioHangChiTiets { get; set; } = new List<GioHangChiTiet>();
        public virtual ICollection<GiayYeuThich> GiayYeuThiches { get; set; } = new List<GiayYeuThich>();
    }

}
