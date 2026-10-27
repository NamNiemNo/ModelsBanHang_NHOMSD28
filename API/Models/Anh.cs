using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Models;

namespace Data.Models
{
    public class Anh
    {
        [Key]
        public Guid AnhId { get; set; }
		public Guid  GiayChiTietId { get; set; }
		public string DuongDan { get; set; }
        public string TenAnh { get; set; }
        public bool TrangThai { get; set; }
       

        public Anh()
        {
            AnhId = Guid.NewGuid();
            TrangThai = true;
        }
		public virtual GiayChiTiet GiayChiTiet { get; set; }


	}
}
