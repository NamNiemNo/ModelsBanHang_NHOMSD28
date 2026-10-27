using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using API.Models.DTO.ChatAI;
using API.Services.AI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace API.Controllers
{
    [ApiController]
    [Route("api/chat-ai")]
    [AllowAnonymous]
    [EnableRateLimiting("ai-chat")]
    public class ChatAIController : ControllerBase
    {
        private readonly IAIChatService _chatService;
        private readonly ILogger<ChatAIController> _logger;

        public ChatAIController(
            IAIChatService chatService,
            ILogger<ChatAIController> logger)
        {
            _chatService = chatService;
            _logger = logger;
        }

        /// <summary>
        /// Gửi câu hỏi đến AI.
        /// </summary>
        [HttpPost("messages")]
        public async Task<ActionResult<ChatResponseDTO>>
            GuiTinNhan(
                [FromBody] ChatRequestDTO request,
                CancellationToken cancellationToken)
        {
            try
            {
                var khachHangId =
                    LayKhachHangIdTuToken();

                var result =
                    await _chatService.GuiTinNhanAsync(
                        request,
                        khachHangId,
                        cancellationToken);

                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(
                    new
                    {
                        message = ex.Message
                    });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(
                    new
                    {
                        message = ex.Message
                    });
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(
                    StatusCodes.Status503ServiceUnavailable,
                    new
                    {
                        message = ex.Message
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Lỗi không xác định khi gửi tin nhắn AI.");

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        message =
                            "Hệ thống AI đang gặp lỗi. " +
                            "Vui lòng thử lại sau."
                    });
            }
        }

        /// <summary>
        /// Lấy lịch sử trò chuyện.
        /// </summary>
        [HttpGet("conversations")]
        public async Task<
            ActionResult<List<ChatConversationDTO>>>
            LayLichSu(
                [FromQuery] Guid phienKhachId,
                CancellationToken cancellationToken)
        {
            if (phienKhachId == Guid.Empty)
            {
                return BadRequest(
                    new
                    {
                        message =
                            "Phiên khách không hợp lệ."
                    });
            }

            var khachHangId =
                LayKhachHangIdTuToken();

            var result =
                await _chatService.LayLichSuAsync(
                    phienKhachId,
                    khachHangId,
                    cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Xóa một cuộc trò chuyện.
        /// </summary>
        [HttpDelete(
            "conversations/{cuocTroChuyenId:guid}")]
        public async Task<IActionResult>
            XoaCuocTroChuyen(
                Guid cuocTroChuyenId,
                [FromQuery] Guid phienKhachId,
                CancellationToken cancellationToken)
        {
            if (phienKhachId == Guid.Empty)
            {
                return BadRequest(
                    new
                    {
                        message =
                            "Phiên khách không hợp lệ."
                    });
            }

            var khachHangId =
                LayKhachHangIdTuToken();

            var daXoa =
                await _chatService.XoaAsync(
                    cuocTroChuyenId,
                    phienKhachId,
                    khachHangId,
                    cancellationToken);

            if (!daXoa)
            {
                return NotFound(
                    new
                    {
                        message =
                            "Không tìm thấy cuộc trò chuyện."
                    });
            }

            return NoContent();
        }

        /// <summary>
        /// Lấy KhachHangId từ JWT nếu khách đã đăng nhập.
        /// Khách chưa đăng nhập sẽ trả về null.
        /// </summary>
        private Guid? LayKhachHangIdTuToken()
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return null;
            }

            if (!User.IsInRole("KhachHang"))
            {
                return null;
            }

            var idValue =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(idValue))
            {
                idValue =
                    User.FindFirstValue(
                        JwtRegisteredClaimNames.Sub);
            }

            if (Guid.TryParse(
                    idValue,
                    out var khachHangId))
            {
                return khachHangId;
            }

            return null;
        }
    }
}