using BlazorKhachHang.Service.IService;
using Data.Models;
using System.Net.Http.Json;

namespace BlazorKhachHang.Service
{
    public class ThuongHieuService : IThuongHieuService
    {
        private readonly HttpClient _httpClient;

        public ThuongHieuService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ThuongHieu>> GetAllAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<ThuongHieu>>("api/ThuongHieu");
        }

        public async Task<ThuongHieu> GetByIdAsync(Guid id)
        {
            return await _httpClient.GetFromJsonAsync<ThuongHieu>($"api/ThuongHieu/{id}");
        }

        public async Task CreateAsync(ThuongHieu obj)
        {
            await _httpClient.PostAsJsonAsync("api/ThuongHieu", obj);
        }

        public async Task UpdateAsync(ThuongHieu obj)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/ThuongHieu/{obj.ThuongHieuId}", obj);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(Guid id)
        {
            await _httpClient.DeleteAsync($"api/ThuongHieu/{id}");
        }
    }
}
