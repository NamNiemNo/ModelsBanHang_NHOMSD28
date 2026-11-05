using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Data.Models
{
    public class Voucher
    {
        [Key]
        public Guid VoucherId { get; set; }

        [Required]
        [StringLength(50)]
        public string TenVoucher { get; set; }

        [Required]
        public DateTime NgayBatDau { get; set; }

        [Required]
        public DateTime NgayKetThuc { get; set; }

        [Range(0, 100)]
        public float PhanTram { get; set; }

        public float SoTienGiam { get; set; }
        public float GiamToiDa { get; set; }
        public float DieuKienToiThieu { get; set; }

        public bool TrangThai { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Tổng lượt sử dụng phải lớn hơn 0.")]
        public int SoLuong { get; set; }
		public virtual ICollection<KhachHangVoucher> KhachHangVouchers { get; set; } = new List<KhachHangVoucher>();




		public void Validate()
        {
            if (NgayBatDau < DateTime.Now)
                throw new Exception("Thời điểm bắt đầu không được ở trong quá khứ.");
            if (NgayKetThuc <= NgayBatDau)
                throw new Exception("Ngày kết thúc phải sau ngày bắt đầu.");
        }
        public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();
    }

}
