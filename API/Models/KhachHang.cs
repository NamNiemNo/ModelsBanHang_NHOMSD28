using API.Models;
using Data.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models

{
    public class KhachHang
    {
        [Key]
        public Guid KhachHangId { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "Họ tên không được để trống")]
        [StringLength(50, ErrorMessage = "Họ tên tối đa 50 ký tự")]
        [RegularExpression(@"^[a-zA-ZÀÁÂÃÈÉÊÌÍÒÓÔÕÙÚĂĐĨŨƠàáâãèéêìíòóôõùúăđĩũơƯĂẠẢẤẦẨẪẬẮẰẲẴẶẸẺẼỀỀỂẾưăạảấầẩẫậắằẳẵặẹẻẽềềểếỄỆỈỊỌỎỐỒỔỖỘỚỜỞỠỢỤỦỨỪễệỉịọỏốồổỗộớờởỡợụủứừỬỮỰỲỴÝỶỸửữựỳỵỷỹ\s]+$", ErrorMessage = "Họ tên chỉ chứa chữ cái và khoảng trắng")]
        public string HoTen { get; set; } 

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [RegularExpression(@"^(09|03)\d{8}$", ErrorMessage = "Số điện thoại phải bắt đầu bằng 09 hoặc 03 và có đúng 10 số")]
        public string SoDienThoai { get; set; } 

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string Email { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string? GioiTinh { get; set; }
        public bool TrangThai { get; set; }
        public DateTime NgayTao { get; set; }
        public DateTime? NgayCapNhatCuoiCung { get; set; }
        public Guid? TaiKhoanId { get; set; }
        public virtual TaiKhoan? TaiKhoan { get; set; }
		public virtual ICollection<KhachHangVoucher> KhachHangVouchers { get; set; } = new List<KhachHangVoucher>();

		public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();
        public virtual ICollection<DiaChiKhachHang> DiaChiKhachHangs { get; set; } = new List<DiaChiKhachHang>();
        public virtual ICollection<GioHang> GioHangs { get; set; } = new List<GioHang>();
        public virtual ICollection<GiayYeuThich> GiayYeuThichs { get; set; } = new List<GiayYeuThich>();
    }
}
