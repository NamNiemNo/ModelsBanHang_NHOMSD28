using System.ComponentModel.DataAnnotations;

namespace API.Models.DTO
{
    public class ChatLieuDTO
    {
        public Guid? ChatLieuId { get; set; }

        [Required(ErrorMessage = "Tên chất liệu không được để trống")]
        [StringLength(50, ErrorMessage = "Không vượt quá 50 ký tự")]
        [RegularExpression(@"^(?!\s*$)[a-zA-ZÀÁÂÃÈÉÊÌÍÒÓÔÕÙÚĂĐĨŨƠƯàáâãèéêìíòóôõùúăđĩũơưẠ-ỹ\s0-9]+$",
            ErrorMessage = "Chỉ chứa chữ cái tiếng Việt, số và khoảng trắng")]
        public string TenChatLieu { get; set; }

        public string MoTa { get; set; }

        public bool TrangThai { get; set; }
    }
}
