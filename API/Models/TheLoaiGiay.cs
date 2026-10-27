using Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class TheLoaiGiay
    {
        [Key]
        public Guid TheLoaiGiayId { get; set; }

        [Required]
        [StringLength(50)]
        [RegularExpression(@"^[a-zA-Z0-9\s]+$")]
        public string TenTheLoai { get; set; }

        public string MoTa { get; set; }

        public bool TrangThai { get; set; }
        public virtual ICollection<Giay> Giays { get; set; } = new List<Giay>();
    }
}
