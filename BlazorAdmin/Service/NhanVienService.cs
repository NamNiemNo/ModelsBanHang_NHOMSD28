using Data.Models;
using BlazorAdmin.Service.IService;
using System.Net.Http;
using System.Net.Http.Json;

namespace BlazorAdmin.Service
{
    public class NhanVienService : INhanVienService
    {
        private readonly HttpClient _httpClient;

        public NhanVienService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task CreateNhanVien(NhanVien nhanVien)
        {
            var response = await _httpClient.PostAsJsonAsync("api/NhanViens", nhanVien);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteNhanVienAsync(Guid NhanVienId)
        {
            var response = await _httpClient.DeleteAsync($"api/NhanViens/{NhanVienId}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<List<NhanVien>> GetAllNhanVienAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<NhanVien>>("api/NhanViens")
                   ?? new List<NhanVien>();
        }

        public async Task<NhanVien> GetByIdNhanVienAsync(Guid NhanVienId)
        {
            return await _httpClient.GetFromJsonAsync<NhanVien>($"api/NhanViens/{NhanVienId}");
        }

        public async Task UpdateNhanVienAsync(NhanVien nhanVien)
        {
            var response = await _httpClient.PutAsJsonAsync(
                $"api/NhanViens/{nhanVien.NhanVienId}",
                nhanVien);

            response.EnsureSuccessStatusCode();
        }

        public async Task<List<NhanVien>> SearchNhanVien(string keyword)
        {
            var result = await _httpClient.GetFromJsonAsync<List<NhanVien>>(
                $"api/NhanViens/search?keyword={Uri.EscapeDataString(keyword)}");

            return result ?? new List<NhanVien>();
        }
    }
}