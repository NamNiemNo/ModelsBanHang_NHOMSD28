using System.ComponentModel.DataAnnotations;

namespace API.Models.DTO.ChatAI
{
    public class ChatRequestDTO
    {
        // Null khi tạo cuộc trò chuyện mới
        public Guid? CuocTroChuyenId { get; set; }

        // Mã nhận diện trình duyệt
        public Guid PhienKhachId { get; set; }

        [Required(ErrorMessage = "Nội dung không được để trống")]
        [StringLength(
            2000,
            MinimumLength = 1,
            ErrorMessage = "Nội dung tối đa 2000 ký tự")]
        public string NoiDung { get; set; }
            = string.Empty;
    }
}