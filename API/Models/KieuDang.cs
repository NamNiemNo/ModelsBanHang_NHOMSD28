using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class KieuDang
    {
        [Key]
        public Guid KieuDangId { get; set; }
        [Required]
        [StringLength(50)]
        [RegularExpression(@"^[a-zA-Z0-9\s]+$", ErrorMessage = "Tên không được chứa ký tự đặc biệt.")]
        public string TenKieuDang { get; set; }
        public string MoTa { get; set; }
        public bool TrangThai { get; set; }
        public virtual ICollection<Giay> Giays { get; set; } = new List<Giay>();
    }
}
