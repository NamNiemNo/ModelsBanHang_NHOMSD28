using BlazorAdmin.Service.IService;
using Data.Models;
using API.Models.DTO; // 👈 Thêm namespace DTO
using System.Net.Http.Json;

namespace BlazorAdmin.Service
{
    public class GiamGiaService : IGiamGiaService
    {
        private readonly HttpClient _httpClient;

        public GiamGiaService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<GiamGia>> GetAllAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<GiamGia>>("api/GiamGia")
                   ?? new List<GiamGia>();
        }

        public async Task<GiamGia> GetByIdAsync(Guid id)
        {
            return await _httpClient.GetFromJsonAsync<GiamGia>($"api/GiamGia/{id}");
        }

        // ✅ Gửi DTO khi thêm mới
        public async Task<HttpResponseMessage> AddAsync(GiamGiaCreateDTO giamGia)
        {
            var response = await _httpClient.PostAsJsonAsync("api/GiamGia", giamGia);
            return response;
        }

        // ✅ Gửi DTO khi cập nhật
        public async Task<HttpResponseMessage> UpdateAsyncReturnResponse(GiamGiaCreateDTO giamGia)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/GiamGia/{giamGia.GiamGiaId}", giamGia);
            return response;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"api/GiamGia/{id}");
            response.EnsureSuccessStatusCode();
            return true;
        }

        public async Task<bool> AddGiayToDotGiamGia(Guid giamGiaId, Guid giayChiTietId)
        {
            var response = await _httpClient.PostAsync($"api/GiamGia/{giamGiaId}/giaychitiet/{giayChiTietId}", null);
            response.EnsureSuccessStatusCode();
            return true;
        }

        public async Task<bool> RemoveGiayFromDotGiamGia(Guid giamGiaId, Guid giayChiTietId)
        {
            var response = await _httpClient.DeleteAsync($"api/GiamGia/{giamGiaId}/giaychitiet/{giayChiTietId}");
            response.EnsureSuccessStatusCode();
            return true;
        }
    }
}
