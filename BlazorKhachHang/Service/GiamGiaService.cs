using BlazorKhachHang.Service.IService;
using Data.Models;
using System.Net.Http.Json;

namespace BlazorKhachHang.Service
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

        public async Task<HttpResponseMessage> AddAsync(GiamGia giamGia)
        {
            var response = await _httpClient.PostAsJsonAsync("api/GiamGia", giamGia);
            return response;
        }

        public async Task<HttpResponseMessage> UpdateAsyncReturnResponse(GiamGia giamGia)
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

        public async Task<bool> AddGiayToDotGiamGia(Guid giamGiaId, Guid giayId)
        {
            var response = await _httpClient.PostAsync($"api/GiamGia/{giamGiaId}/giay/{giayId}", null);
            response.EnsureSuccessStatusCode();
            return true;
        }

        public async Task<bool> RemoveGiayFromDotGiamGia(Guid giamGiaId, Guid giayId)
        {
            var response = await _httpClient.DeleteAsync($"api/GiamGia/{giamGiaId}/giay/{giayId}");
            response.EnsureSuccessStatusCode();
            return true;
        }
    }
}
