using API.Models;
using API.Models.DTO.TraHang;
using BlazorKhachHang.Service.IService;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace BlazorKhachHang.Service
{
    public class ReturnService : IReturnService
    {
        private readonly HttpClient _httpClient;

        public ReturnService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

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

            var response = await _httpClient.GetAsync(queryBuilder.ToString());
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[Blazor] SearchHoaDon lỗi: {(int)response.StatusCode} - {response.ReasonPhrase}");
                return new List<ReturnDTO>();
            }

            return await response.Content.ReadFromJsonAsync<List<ReturnDTO>>() ?? new List<ReturnDTO>();
        }

        public async Task<ReturnDTO> GetReturnInfoAsync(Guid hoaDonId)
        {
            var response = await _httpClient.GetAsync($"api/Return/{hoaDonId}");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[Blazor] GetReturnInfo lỗi: {(int)response.StatusCode} - {response.ReasonPhrase}");
                return new ReturnDTO();
            }

            return await response.Content.ReadFromJsonAsync<ReturnDTO>() ?? new ReturnDTO();
        }

        public async Task<List<ReturnHistoryDTO>> GetReturnHistoryAsync(Guid hoaDonId)
        {
            var response = await _httpClient.GetAsync($"api/Return/history/{hoaDonId}");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[Blazor] GetReturnHistory lỗi: {(int)response.StatusCode} - {response.ReasonPhrase}");
                return new List<ReturnHistoryDTO>();
            }

            return await response.Content.ReadFromJsonAsync<List<ReturnHistoryDTO>>() ?? new List<ReturnHistoryDTO>();
        }

        public async Task<List<ValidateReturnResultDTO>> ValidateReturnAsync(CreateReturnDTO request)
        {
            try
            {
                Console.WriteLine($"[Blazor] Sending ValidateReturn request: HoaDonId={request.HoaDonId}, ItemsCount={request.Items?.Count ?? 0}");
                var response = await _httpClient.PostAsJsonAsync("api/Return/validate", request);
                Console.WriteLine($"[Blazor] ValidateReturn response: StatusCode={(int)response.StatusCode}, Reason={response.ReasonPhrase}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"[Blazor] ValidateReturn lỗi: Status={(int)response.StatusCode}, Content={errorContent}");
                    return new List<ValidateReturnResultDTO>
                    {
                        new ValidateReturnResultDTO
                        {
                            CanReturn = false,
                            LyDoTuChoi = $"Lỗi từ server: {(int)response.StatusCode} - {errorContent}"
                        }
                    };
                }

                var result = await response.Content.ReadFromJsonAsync<List<ValidateReturnResultDTO>>();
                Console.WriteLine($"[Blazor] ValidateReturn result: Count={(result != null ? result.Count : 0)}");
                return result ?? new List<ValidateReturnResultDTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Blazor] Lỗi ValidateReturnAsync: {ex.Message}");
                return new List<ValidateReturnResultDTO>
                {
                    new ValidateReturnResultDTO
                    {
                        CanReturn = false,
                        LyDoTuChoi = $"Lỗi khi gọi API: {ex.Message}"
                    }
                };
            }
        }

        public async Task<bool> ProcessReturnAsync(CreateReturnDTO request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/Return", request);
                Console.WriteLine($"[Blazor] ProcessReturn response: StatusCode={(int)response.StatusCode}, Reason={response.ReasonPhrase}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"[Blazor] ProcessReturn lỗi: {(int)response.StatusCode} - {errorContent}");
                    return false;
                }

                return await response.Content.ReadFromJsonAsync<bool>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Blazor] Lỗi ProcessReturnAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteReturnAsync(Guid hoaDonChiTietId)
        {
            var response = await _httpClient.DeleteAsync($"api/Return/{hoaDonChiTietId}");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[Blazor] DeleteReturn lỗi: {(int)response.StatusCode} - {response.ReasonPhrase}");
                return false;
            }

            return await response.Content.ReadFromJsonAsync<bool>();
        }
        public async Task<List<TraHang>> GetYeuCauByHoaDonAsync(Guid hoaDonId)
        {
            return await _httpClient.GetFromJsonAsync<List<TraHang>>($"api/Return/requests/{hoaDonId}") ?? new List<TraHang>();
        }
        public async Task<List<ReturnHistoryDTO>> GetReturnHistory(Guid hoaDonId)
        {
            return await _httpClient.GetFromJsonAsync<List<ReturnHistoryDTO>>(
                $"api/Return/history/{hoaDonId}");
        }

    }
}