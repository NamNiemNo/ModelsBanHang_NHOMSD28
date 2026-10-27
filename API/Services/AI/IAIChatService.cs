using API.Models.DTO.ChatAI;

namespace API.Services.AI
{
    public interface IAIChatService
    {
        /// <summary>
        /// Gửi tin nhắn, gọi OpenAI và lưu lịch sử vào SQL Server.
        /// </summary>
        Task<ChatResponseDTO> GuiTinNhanAsync(
            ChatRequestDTO request,
            Guid? khachHangId,
            CancellationToken cancellationToken);

        /// <summary>
        /// Lấy danh sách cuộc trò chuyện của khách hàng
        /// hoặc khách chưa đăng nhập.
        /// </summary>
        Task<List<ChatConversationDTO>> LayLichSuAsync(
            Guid phienKhachId,
            Guid? khachHangId,
            CancellationToken cancellationToken);

        /// <summary>
        /// Xóa mềm một cuộc trò chuyện.
        /// </summary>
        Task<bool> XoaAsync(
            Guid cuocTroChuyenId,
            Guid phienKhachId,
            Guid? khachHangId,
            CancellationToken cancellationToken);
    }
}