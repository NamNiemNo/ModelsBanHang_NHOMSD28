using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models
{
	public class KhachHangVoucher
	{
		[Key]
		public Guid Id { get; set; } = Guid.NewGuid();

		[Required]
		public Guid KhachHangId { get; set; }

		[Required]
		public Guid VoucherId { get; set; }

		public DateTime NgayTao { get; set; } = DateTime.Now;

		public bool DaSuDung { get; set; } = false;

		// Navigation properties
		public virtual KhachHang KhachHang { get; set; }
		public virtual Voucher Voucher { get; set; }
	}
}
