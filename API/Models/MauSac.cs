using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class MauSac
    {
        [Key]
        public Guid MauSacId { get; set; }

        [Required]
        [StringLength(50)]
        [RegularExpression(@"^[a-zA-Z0-9\s]+$")]
        public string TenMau { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string MoTa { get; set; }

        public bool TrangThai { get; set; }
        public virtual ICollection<GiayChiTiet> GiayChiTiets { get; set; } = new List<GiayChiTiet>();

    }
}
