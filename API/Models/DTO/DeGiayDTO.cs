using System.ComponentModel.DataAnnotations;

namespace API.Models.DTO
{
    public class DeGiayDTO
    {
        public Guid DeGiayId { get; set; }
        [Required]
        [StringLength(50)]
        [RegularExpression(@"^[a-zA-ZÀÁÂÃÈÉÊÌÍÒÓÔÕÙÚĂĐĨŨƠƯàáâãèéêìíòóôõùúăđĩũơưẠ-ỹ\s0-9]+$", ErrorMessage = "Tên chỉ được chứa chữ cái tiếng Việt, số và khoảng trắng")]
        public string TenDeGiay { get; set; }
        public string MoTa { get; set; }
        public bool TrangThai { get; set; }
        public decimal GiaBan { get; set; }
    }
}
