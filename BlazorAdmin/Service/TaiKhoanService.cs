using API.Models.DTO;
using BlazorAdmin.Service.IService;
using Data.Models;
using Microsoft.JSInterop;
using System.Text.Json;

namespace BlazorAdmin.Service
{
    public class TaiKhoanService
        : ITaiKhoanService
    {
        private readonly HttpClient _httpClient;
        private readonly IJSRuntime _jsRuntime;

        public TaiKhoanService(
            HttpClient httpClient,
            IJSRuntime jsRuntime)
        {
            _httpClient = httpClient;
            _jsRuntime = jsRuntime;
        }

        public async Task CreateTaiKhoanAsync(
            TaiKhoan tk)
        {
            var response =
                await _httpClient.PostAsJsonAsync(
                    "api/TaiKhoans",
                    tk);

            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteTaiKhoanAsync(
            Guid id)
        {
            var response =
                await _httpClient.DeleteAsync(
                    $"api/TaiKhoans/{id}");

            response.EnsureSuccessStatusCode();
        }

        public async Task<List<TaiKhoan>>
            GetAllTaiKhoanAsync()
        {
            return await _httpClient
                       .GetFromJsonAsync<List<TaiKhoan>>(
                           "api/TaiKhoans")
                   ?? new List<TaiKhoan>();
        }

        public async Task<TaiKhoan?>
            GetByIdTaiKhoanAsync(Guid id)
        {
            try
            {
                var response =
                    await _httpClient.GetAsync(
                        $"api/TaiKhoans/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var content =
                    await response.Content
                        .ReadAsStringAsync();

                if (string.IsNullOrWhiteSpace(content))
                {
                    return null;
                }

                return JsonSerializer
                    .Deserialize<TaiKhoan>(
                        content,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive =
                                true
                        });
            }
            catch
            {
                return null;
            }
        }

        public async Task UpdateTaiKhoanAsync(
            TaiKhoan tk)
        {
            var response =
                await _httpClient.PutAsJsonAsync(
                    $"api/TaiKhoans/{tk.TaikhoanId}",
                    tk);

            response.EnsureSuccessStatusCode();
        }

        public async Task<LoginResponseDto>
            GetByIdChucVuAsync(
                string username,
                string password)
        {
            try
            {
                var encodedUsername =
                    Uri.EscapeDataString(username);

                var encodedPassword =
                    Uri.EscapeDataString(password);

                var response =
                    await _httpClient.GetAsync(
                        $"api/TaiKhoans/login" +
                        $"?username={encodedUsername}" +
                        $"&pass={encodedPassword}");

                var result =
                    await response.Content
                        .ReadFromJsonAsync<LoginResponseDto>();

                if (!response.IsSuccessStatusCode ||
                    result == null)
                {
                    return result ??
                           new LoginResponseDto
                           {
                               IsSuccess = false,
                               Message =
                                   "Tên đăng nhập hoặc mật khẩu không đúng."
                           };
                }

                /*
                 * Lưu JWT do API cấp.
                 * Login.razor hiện tại không cần sửa.
                 */
                if (result.IsSuccess &&
                    !string.IsNullOrWhiteSpace(
                        result.Token))
                {
                    await _jsRuntime.InvokeVoidAsync(
                        "localStorage.setItem",
                        "authToken",
                        result.Token);
                }

                return result;
            }
            catch
            {
                return new LoginResponseDto
                {
                    IsSuccess = false,
                    Message =
                        "Không thể kết nối đến API."
                };
            }
        }

        public async Task<TaiKhoan?>
            GetByUsernameAsync(string username)
        {
            try
            {
                var encodedUsername =
                    Uri.EscapeDataString(username);

                var response =
                    await _httpClient.GetAsync(
                        $"api/TaiKhoans/username/{encodedUsername}");

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                return await response.Content
                    .ReadFromJsonAsync<TaiKhoan>();
            }
            catch
            {
                return null;
            }
        }
    }
}