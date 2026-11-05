using System.ComponentModel.DataAnnotations;

namespace API.Models.DTO
{
    public class RegisterCustomerDTO
    {
        [Required(ErrorMessage = "Vui lòng nhập họ tên.")]
        [StringLength(50, ErrorMessage = "Họ tên tối đa 50 ký tự.")]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập.")]
        [StringLength(30, MinimumLength = 4,
            ErrorMessage = "Tên đăng nhập phải từ 4 đến 30 ký tự.")]
        [RegularExpression(@"^[a-zA-Z0-9]+$",
            ErrorMessage = "Tên đăng nhập chỉ được chứa chữ cái không dấu và số.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
        [StringLength(100, MinimumLength = 6,
            ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu.")]
        [Compare(nameof(Password),
            ErrorMessage = "Mật khẩu xác nhận không khớp.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập email.")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
        [RegularExpression(@"^(09|03)\d{8}$",
            ErrorMessage = "Số điện thoại phải bắt đầu bằng 09 hoặc 03 và có đúng 10 số.")]
        public string SoDienThoai { get; set; } = string.Empty;

        public DateTime? NgaySinh { get; set; }

        public string? GioiTinh { get; set; }
    }
}