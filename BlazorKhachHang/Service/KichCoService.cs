using API.IService;
using API.Models.DTO;
using BlazorKhachHang.Service.IService;
using Data.Models;
using System.Net.Http.Json;

namespace BlazorKhachHang.Service
{
    public class KichCoService : IKichCoService
    {
        private readonly HttpClient _httpClient;

        public KichCoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<KichCoDTO>> GetAllAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<KichCoDTO>>("api/KichCo")
                   ?? new List<KichCoDTO>();
        }

        public async Task<KichCoDTO> GetByIdAsync(Guid id)
        {
            return await _httpClient.GetFromJsonAsync<KichCoDTO>($"api/KichCo/{id}");
        }

        public async Task CreateAsync(KichCoDTO obj)
        {
            var response = await _httpClient.PostAsJsonAsync("api/KichCo", obj);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateAsync(KichCoDTO obj)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/KichCo/{obj.KichCoId}", obj);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"api/KichCo/{id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<List<KichCoDTO>> SearchAsync(string keyword)
        {
            return await _httpClient.GetFromJsonAsync<List<KichCoDTO>>($"api/KichCo/search?keyword={keyword}")
                   ?? new List<KichCoDTO>();
        }
    }
}