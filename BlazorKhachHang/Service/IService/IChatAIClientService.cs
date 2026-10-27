using API.Models.DTO.ChatAI;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorClient.Services
{
    /// <summary>
    /// Định nghĩa các chức năng giao tiếp giữa
    /// giao diện Blazor và Chat AI API.
    /// </summary>
    public interface IChatAIClientService
    {
        /// <summary>
        /// Gửi tin nhắn của khách hàng đến trợ lý AI.
        /// </summary>
        Task<ChatResponseDTO> GuiTinNhanAsync(ChatRequestDTO request,CancellationToken cancellationToken = default);

        /// <summary>
        /// Lấy danh sách cuộc trò chuyện của
        /// phiên trình duyệt hoặc khách hàng đã đăng nhập.
        /// </summary>
        Task<List<ChatConversationDTO>> LayLichSuAsync(Guid phienKhachId,CancellationToken cancellationToken = default);

        /// <summary>
        /// Xóa một cuộc trò chuyện thuộc phiên hiện tại.
        /// </summary>
        Task<bool> XoaCuocTroChuyenAsync(Guid cuocTroChuyenId, Guid phienKhachId, CancellationToken cancellationToken = default);
    }
}