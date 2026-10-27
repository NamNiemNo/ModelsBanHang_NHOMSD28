using API.Models.DTO.ChatAI;
using Blazored.LocalStorage;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorClient.Services
{
    /// <summary>
    /// Gọi các endpoint Chat AI từ giao diện Blazor.
    /// OpenAI API key không được sử dụng trong service này.
    /// </summary>
    public class ChatAIClientService
        : IChatAIClientService
    {
        private static readonly string[] TokenKeys =
        {
            "authToken",
            "accessToken",
            "token"
        };

        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;

        public ChatAIClientService(
            HttpClient httpClient,
            ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
        }

        /// <summary>
        /// Gửi câu hỏi của khách hàng tới Chat AI API.
        /// </summary>
        public async Task<ChatResponseDTO>
            GuiTinNhanAsync(
                ChatRequestDTO request,
                CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);

            using var httpRequest =
                new HttpRequestMessage(
                    HttpMethod.Post,
                    "api/chat-ai/messages")
                {
                    Content =
                        JsonContent.Create(request)
                };

            using var response =
                await GuiYeuCauAsync(
                    httpRequest,
                    cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(
                    await DocThongBaoLoiAsync(
                        response,
                        cancellationToken));
            }

            var result =
                await response.Content
                    .ReadFromJsonAsync<ChatResponseDTO>(
                        cancellationToken:
                            cancellationToken);

            if (result == null)
            {
                throw new InvalidOperationException(
                    "API Chat AI không trả về dữ liệu.");
            }

            return result;
        }

        /// <summary>
        /// Lấy lịch sử trò chuyện của phiên khách hiện tại.
        /// </summary>
        public async Task<List<ChatConversationDTO>>
            LayLichSuAsync(
                Guid phienKhachId,
                CancellationToken cancellationToken = default)
        {
            if (phienKhachId == Guid.Empty)
            {
                throw new ArgumentException(
                    "Phiên khách không hợp lệ.",
                    nameof(phienKhachId));
            }

            var url =
                "api/chat-ai/conversations" +
                $"?phienKhachId={phienKhachId:D}";

            using var httpRequest =
                new HttpRequestMessage(
                    HttpMethod.Get,
                    url);

            using var response =
                await GuiYeuCauAsync(
                    httpRequest,
                    cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(
                    await DocThongBaoLoiAsync(
                        response,
                        cancellationToken));
            }

            var result =
                await response.Content
                    .ReadFromJsonAsync<
                        List<ChatConversationDTO>>(
                        cancellationToken:
                            cancellationToken);

            return result ??
                new List<ChatConversationDTO>();
        }

        /// <summary>
        /// Xóa một cuộc trò chuyện của phiên hiện tại.
        /// </summary>
        public async Task<bool>
            XoaCuocTroChuyenAsync(
                Guid cuocTroChuyenId,
                Guid phienKhachId,
                CancellationToken cancellationToken = default)
        {
            if (cuocTroChuyenId == Guid.Empty)
            {
                throw new ArgumentException(
                    "Cuộc trò chuyện không hợp lệ.",
                    nameof(cuocTroChuyenId));
            }

            if (phienKhachId == Guid.Empty)
            {
                throw new ArgumentException(
                    "Phiên khách không hợp lệ.",
                    nameof(phienKhachId));
            }

            var url =
                "api/chat-ai/conversations/" +
                $"{cuocTroChuyenId:D}" +
                $"?phienKhachId={phienKhachId:D}";

            using var httpRequest =
                new HttpRequestMessage(
                    HttpMethod.Delete,
                    url);

            using var response =
                await GuiYeuCauAsync(
                    httpRequest,
                    cancellationToken);

            if (response.StatusCode ==
                HttpStatusCode.NotFound)
            {
                return false;
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(
                    await DocThongBaoLoiAsync(
                        response,
                        cancellationToken));
            }

            return true;
        }

        /// <summary>
        /// Gửi yêu cầu tới API nội bộ.
        /// Nếu khách đã đăng nhập, JWT sẽ được gắn vào yêu cầu.
        /// </summary>
        private async Task<HttpResponseMessage>
            GuiYeuCauAsync(
                HttpRequestMessage request,
                CancellationToken cancellationToken)
        {
            await GanTokenDangNhapAsync(
                request,
                cancellationToken);

            try
            {
                return await _httpClient.SendAsync(
                    request,
                    HttpCompletionOption
                        .ResponseHeadersRead,
                    cancellationToken);
            }
            catch (OperationCanceledException)
                when (!cancellationToken
                    .IsCancellationRequested)
            {
                throw new InvalidOperationException(
                    "API phản hồi quá lâu. " +
                    "Vui lòng thử lại.");
            }
            catch (HttpRequestException)
            {
                throw new InvalidOperationException(
                    "Không thể kết nối tới API. " +
                    "Hãy kiểm tra project API đã được chạy.");
            }
        }

        /// <summary>
        /// Gắn JWT của khách hàng vào request nếu có.
        /// Khách chưa đăng nhập vẫn có thể sử dụng Chat AI.
        /// </summary>
        private async Task GanTokenDangNhapAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var token =
                await LayTokenDangNhapAsync(
                    cancellationToken);

            if (string.IsNullOrWhiteSpace(token))
            {
                return;
            }

            token = token.Trim();

            if (token.StartsWith(
                    "Bearer ",
                    StringComparison.OrdinalIgnoreCase))
            {
                token =
                    token["Bearer ".Length..]
                        .Trim();
            }

            if (string.IsNullOrWhiteSpace(token))
            {
                return;
            }

            request.Headers.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    token);
        }

        /// <summary>
        /// Tìm JWT theo các tên thường được sử dụng
        /// trong LocalStorage của dự án.
        /// </summary>
        private async Task<string?>
            LayTokenDangNhapAsync(
                CancellationToken cancellationToken)
        {
            try
            {
                foreach (var tokenKey in TokenKeys)
                {
                    var token =
                        await _localStorage
                            .GetItemAsync<string>(
                                tokenKey,
                                cancellationToken);

                    if (!string.IsNullOrWhiteSpace(token))
                    {
                        return token;
                    }
                }
            }
            catch (InvalidOperationException)
            {
                // LocalStorage chưa sẵn sàng khi prerender.
                // Người dùng vẫn được sử dụng như khách vãng lai.
            }
            catch (JSException)
            {
                // Không đọc được LocalStorage.
                // Tiếp tục gửi yêu cầu với phiên khách.
            }

            return null;
        }

        /// <summary>
        /// Đọc thông báo lỗi do API trả về.
        /// </summary>
        private static async Task<string>
            DocThongBaoLoiAsync(
                HttpResponseMessage response,
                CancellationToken cancellationToken)
        {
            try
            {
                var json =
                    await response.Content
                        .ReadAsStringAsync(
                            cancellationToken);

                if (!string.IsNullOrWhiteSpace(json))
                {
                    using var document =
                        JsonDocument.Parse(json);

                    if (document.RootElement
                            .TryGetProperty(
                                "message",
                                out var messageElement))
                    {
                        var message =
                            messageElement.GetString();

                        if (!string.IsNullOrWhiteSpace(
                                message))
                        {
                            return message;
                        }
                    }
                }
            }
            catch (JsonException)
            {
                // Phản hồi không phải JSON.
            }

            return response.StatusCode switch
            {
                HttpStatusCode.BadRequest =>
                    "Nội dung gửi tới AI không hợp lệ.",

                HttpStatusCode.Unauthorized =>
                    "Phiên đăng nhập đã hết hạn.",

                HttpStatusCode.Forbidden =>
                    "Bạn không có quyền thực hiện thao tác này.",

                HttpStatusCode.NotFound =>
                    "Không tìm thấy cuộc trò chuyện.",

                HttpStatusCode.TooManyRequests =>
                    "Bạn đang gửi tin nhắn quá nhanh. " +
                    "Vui lòng chờ một lát rồi thử lại.",

                HttpStatusCode.ServiceUnavailable =>
                    "Dịch vụ AI đang tạm thời không sẵn sàng.",

                _ =>
                    "Chat AI gặp lỗi. Vui lòng thử lại sau."
            };
        }
    }
}