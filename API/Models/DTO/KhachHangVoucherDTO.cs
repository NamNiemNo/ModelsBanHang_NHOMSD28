using System;

namespace API.Models.DTO
{
	public class KhachHangVoucherDTO
	{
		public Guid Id { get; set; } // Có thể dùng làm khóa chính khi sửa/xóa

		public Guid KhachHangId { get; set; }

		public Guid VoucherId { get; set; }

		public bool DaSuDung { get; set; }

		public DateTime NgayTao { get; set; } = DateTime.Now;

		// Optional: để hiển thị tên trong bảng danh sách
		public string? TenKhachHang { get; set; }
		public string? TenVoucher { get; set; }
	}
}
