using Data.Models;
using System.ComponentModel.DataAnnotations;

namespace Data.Models
{
    public class NhanVien
    {
        [Key]
        public Guid NhanVienId { get; set; } = Guid.NewGuid();

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

        public DateTime NgaySinh { get; set; }
        public bool TrangThai { get; set; }
        public Guid? ChucVuId { get; set; }
        public Guid? TaikhoanId { get; set; }

        public DateTime NgayCapNhatCuoiCung { get; set; }

        public ChucVu? ChucVu { get; set; }
        public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();
        public virtual TaiKhoan? TaiKhoan { get; set; }
    }
}
