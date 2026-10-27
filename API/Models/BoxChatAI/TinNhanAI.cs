using System.ComponentModel.DataAnnotations;

namespace API.Models.BoxChatAI
{
    public class TinNhanAI
    {
        [Key]
        public Guid TinNhanAIId { get; set; }
            = Guid.NewGuid();

        public Guid CuocTroChuyenAIId { get; set; }

        // user hoặc assistant
        [Required]
        [MaxLength(20)]
        public string VaiTro { get; set; }
            = "user";

        [Required]
        public string NoiDung { get; set; }
            = string.Empty;

        [MaxLength(100)]
        public string? ModelAI { get; set; }

        public DateTime ThoiGian { get; set; }
            = DateTime.UtcNow;

        public virtual CuocTroChuyenAI CuocTroChuyen { get; set; }
            = null!;
    }
}