namespace API.Models.DTO.ChatAI
{
    public class ChatConversationDTO
    {
        public Guid CuocTroChuyenId { get; set; }

        public string TieuDe { get; set; }
            = string.Empty;

        public DateTime NgayCapNhat { get; set; }

        public List<ChatMessageDTO> TinNhans { get; set; }
            = new List<ChatMessageDTO>();
    }
}