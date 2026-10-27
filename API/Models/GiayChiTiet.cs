using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Models;

namespace Data.Models
{
    public class GiayChiTiet
    {
        [Key]
        public Guid GiayChiTietId { get; set; }

        [Required]
        public Guid GiayId { get; set; }

       
        public Guid ? KichCoId { get; set; }
        public Guid ? MauSacId { get; set; }
       
        public int SoLuongCon { get; set; }
        public int SoLuongDat { get; set; }
        public DateTime NgayTao { get; set; }
        public DateTime NgaySua { get; set; }
        public float Gia { get; set; }

        public string ? MoTa { get; set; }
        public bool TrangThai { get; set; }

        public virtual Giay Giay { get; set; }
        public virtual KichCo ? KichCo { get; set; }
        public virtual MauSac ? MauSac { get; set; }
		public virtual ICollection<Anh> Anhs { get; set; } = new List<Anh>();

	}
}