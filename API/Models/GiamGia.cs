using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class GiamGia : IValidatableObject
    {
        [Key]
        public Guid GiamGiaId { get; set; }

        [Required]
        [StringLength(50)]
        [RegularExpression(@"^[a-zA-ZÀÁÂÃÈÉÊÌÍÒÓÔÕÙÚĂĐĨŨƠƯàáâãèéêìíòóôõùúăđĩũơưẠ-ỹ\s0-9]+$", ErrorMessage = "Tên chỉ được chứa chữ cái tiếng Việt, số và khoảng trắng")]
        public string TenGiamGia { get; set; }

        public string? SanPhamKhuyenMai { get; set; } // ✅ cho phép null

        public float PhanTramKhuyenMai { get; set; }

        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }

        public bool TrangThai { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (NgayBatDau.Date < DateTime.Now.Date)
            {
                yield return new ValidationResult("Ngày bắt đầu không được ở trong quá khứ.", new[] { nameof(NgayBatDau) });
            }
        }
        public virtual ICollection<GiayDotGiamGia> GiayDotGiamGias { get; set; } = new List<GiayDotGiamGia>();

    }


}
