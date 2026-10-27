namespace API.Models.DTO.ChatAI
{
    public class ChatResponseDTO
    {
        public Guid CuocTroChuyenId { get; set; }

        public ChatMessageDTO TinNhan { get; set; }
            = new ChatMessageDTO();

        public List<ChatProductDTO> SanPhams { get; set; }
            = new List<ChatProductDTO>();
    }
}