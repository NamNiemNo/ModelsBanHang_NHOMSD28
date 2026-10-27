using System.ComponentModel.DataAnnotations;

namespace API.Models.DTO
{
    public class InputModel
    {
        [Required(ErrorMessage = "Chưa nhập tên đăng nhập")]
        [Display(Name = "UserName")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Chưa nhập mật khẩu")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;
        public bool RememberMe { get; set; } = false;
    }
}
