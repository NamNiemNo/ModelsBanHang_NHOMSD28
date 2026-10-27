using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class GiayDotGiamGia
    {
        [Key]
        public Guid GiayDotGiamGiaId { get; set; }

        public Guid? GiamGiaId { get; set; }
        public Guid? GiayChiTietId { get; set; }

        public virtual GiamGia? GiamGia { get; set; }  // Cho phép null
        public virtual GiayChiTiet? GiayChiTiet { get; set; }  // Cho phép null
    }

}
