namespace API.Models.DTO.ChatAI
{
    public class ChatMessageDTO
    {
        public Guid TinNhanId { get; set; }

        // user hoặc assistant
        public string VaiTro { get; set; }
            = string.Empty;

        public string NoiDung { get; set; }
            = string.Empty;

        public DateTime ThoiGian { get; set; }
    }
}