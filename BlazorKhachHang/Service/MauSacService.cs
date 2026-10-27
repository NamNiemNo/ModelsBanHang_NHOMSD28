using API.Models.DTO;
using BlazorKhachHang.Service.IService;
using System.Net.Http.Json;

namespace BlazorKhachHang.Service
{
    public class MauSacService : IMauSacService
    {
        private readonly HttpClient _httpClient;

        public MauSacService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }


        public async Task<List<MauSacDTO>> GetAllAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<MauSacDTO>>("api/MauSac")
                   ?? new List<MauSacDTO>();
        }

        public async Task<MauSacDTO> GetByIdAsync(Guid id)
        {
            return await _httpClient.GetFromJsonAsync<MauSacDTO>($"api/MauSac/{id}");
        }

        public async Task CreateAsync(MauSacDTO obj)
        {
            var response = await _httpClient.PostAsJsonAsync("api/MauSac", obj);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateAsync(MauSacDTO obj)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/MauSac/{obj.MauSacId}", obj);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"api/MauSac/{id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<List<MauSacDTO>> SearchAsync(string keyword)
        {
            return await _httpClient.GetFromJsonAsync<List<MauSacDTO>>($"api/MauSac/search?keyword={keyword}")
                   ?? new List<MauSacDTO>();
        }
    }
}
