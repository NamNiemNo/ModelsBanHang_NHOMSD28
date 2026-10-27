using API.Models;
using API.Models.DTO.TraHang;
using BlazorAdmin.Service.IService;
using Data.Models;
using System.Net.Http.Json;
using System.Text;

namespace BlazorAdmin.Service
{
    public class ReturnService : IReturnService
    {
        private readonly HttpClient _httpClient;

        public ReturnService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // --- CÁC PHƯƠNG THỨC HIỆN CÓ (Giữ nguyên logic của bạn) ---
        public async Task<List<ReturnDTO>> SearchHoaDonAsync(string maHoaDon = null, string tenKhachHang = null, string sdt = null, DateTime? ngayTao = null, string trangThai = null)
        {
            var queryBuilder = new StringBuilder("api/Return?");
            var parameters = new List<string>();
            if (!string.IsNullOrEmpty(maHoaDon)) parameters.Add($"maHoaDon={Uri.EscapeDataString(maHoaDon)}");
            if (!string.IsNullOrEmpty(tenKhachHang)) parameters.Add($"tenKhachHang={Uri.EscapeDataString(tenKhachHang)}");
            if (!string.IsNullOrEmpty(sdt)) parameters.Add($"sdt={Uri.EscapeDataString(sdt)}");
            if (ngayTao.HasValue) parameters.Add($"ngayTao={Uri.EscapeDataString(ngayTao.Value.ToString("o"))}");
            if (!string.IsNullOrEmpty(trangThai)) parameters.Add($"trangThai={Uri.EscapeDataString(trangThai)}");
            queryBuilder.Append(string.Join("&", parameters));

            return await _httpClient.GetFromJsonAsync<List<ReturnDTO>>(queryBuilder.ToString()) ?? new List<ReturnDTO>();
        }

        public async Task<ReturnDTO> GetReturnInfoAsync(Guid hoaDonId)
        {
            return await _httpClient.GetFromJsonAsync<ReturnDTO>($"api/Return/{hoaDonId}") ?? new ReturnDTO();
        }

        public async Task<List<ReturnHistoryDTO>> GetReturnHistoryAsync(Guid hoaDonId)
        {
            return await _httpClient.GetFromJsonAsync<List<ReturnHistoryDTO>>($"api/Return/history/{hoaDonId}") ?? new List<ReturnHistoryDTO>();
        }

        // --- CÁC PHƯƠNG THỨC MỚI CẦN BỔ SUNG ĐỂ FIX LỖI ---

        // Fix lỗi: 'ReturnService' does not implement 'GetHoaDonByIdAsync'
        public async Task<HoaDon?> GetHoaDonByIdAsync(Guid hoaDonId)
        {
            try
            {
                // Gọi tới endpoint API trả về object HoaDon
                return await _httpClient.GetFromJsonAsync<HoaDon>($"api/Return/invoice/{hoaDonId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Blazor] GetHoaDonByIdAsync lỗi: {ex.Message}");
                return null;
            }
        }

        // Fix lỗi: Triển khai logic phê duyệt cho Admin
        public async Task<bool> XacNhanTraHangAsync(Guid traHangId)
        {
            var response = await _httpClient.PostAsync($"api/Return/approve/{traHangId}", null);
            return response.IsSuccessStatusCode;
        }

        // Fix lỗi: Triển khai logic từ chối cho Admin
        public async Task<bool> TuChoiTraHangAsync(Guid traHangId, string lyDo)
        {
            // Gửi lý do qua body dưới dạng chuỗi JSON
            var response = await _httpClient.PostAsJsonAsync($"api/Return/reject/{traHangId}", lyDo);
            return response.IsSuccessStatusCode;
        }

        // --- CÁC PHƯƠNG THỨC VALIDATE & PROCESS ---
        public async Task<List<ValidateReturnResultDTO>> ValidateReturnAsync(CreateReturnDTO request)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Return/validate", request);
            return await response.Content.ReadFromJsonAsync<List<ValidateReturnResultDTO>>() ?? new List<ValidateReturnResultDTO>();
        }

        public async Task<bool> ProcessReturnAsync(CreateReturnDTO request)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Return", request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteReturnAsync(Guid hoaDonChiTietId)
        {
            var response = await _httpClient.DeleteAsync($"api/Return/{hoaDonChiTietId}");
            return response.IsSuccessStatusCode;
        }
        public async Task<List<TraHang>> GetYeuCauByHoaDonAsync(Guid hoaDonId)
        {
            return await _httpClient.GetFromJsonAsync<List<TraHang>>($"api/Return/requests/{hoaDonId}") ?? new List<TraHang>();
        }
    }
}