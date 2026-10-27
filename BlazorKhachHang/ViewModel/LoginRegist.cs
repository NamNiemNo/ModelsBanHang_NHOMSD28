using System.ComponentModel.DataAnnotations;

namespace BlazorKhachHang.ViewModel
{
    public class LoginRegist
    {
        [Required(ErrorMessage = "Bạn chưa nhập tên!")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Tên đăng nhập không được chứa ký tự đặc biệt")]
        public string Ten { get; set; }
        [Required(ErrorMessage ="Bạn chưa nhập địa chỉ email!")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Tên đăng nhập không được chứa ký tự đặc biệt")]
        public string UserName { get; set; }
        [Required(ErrorMessage ="Bạn chưa nhập mật khẩu!")]
        public string Password { get; set; }
        [Compare(nameof(Password), ErrorMessage = "Mật khẩu nhập lại không khớp")]
        public string RePassword { get; set; }
        public bool Rememberme { get; set; }

    }
}
