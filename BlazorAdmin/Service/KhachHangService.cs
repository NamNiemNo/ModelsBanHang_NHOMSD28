using Azure;
using BlazorAdmin.Components.Pages;
using BlazorAdmin.Service.IService;
using Data.Models;
using System.Text.Json;

namespace BlazorAdmin.Service
{
    // hahaha
    public class KhachHangService : IKhachHangService
    {
        private readonly HttpClient _httpClient;

        public KhachHangService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<KhachHang> Create(KhachHang khachHang)
        {
            try
            {
                var re = await _httpClient.PostAsJsonAsync("api/KhachHang", khachHang);
                var content = await re.Content.ReadAsStringAsync();
                if (re.IsSuccessStatusCode)
                {
                    return JsonSerializer.Deserialize<KhachHang>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
                else
                {
                    return default;
                }
            }
            catch (Exception)
            {

                return default; 
            }
            
        }

        public async Task Delete(Guid id)
        {
            await _httpClient.DeleteAsync("api/KhachHang/" + id);
        }

        public async Task<List<KhachHang>> GetAll()
        {
            return await _httpClient.GetFromJsonAsync<List<KhachHang>>("api/KhachHang") ?? new();
        }

        public async Task<KhachHang?> GetById(Guid id)
        {
            return await _httpClient.GetFromJsonAsync<KhachHang>($"api/KhachHang/{id}");
        }

        public async Task Update(KhachHang khachHang)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/KhachHang/{khachHang.KhachHangId}", khachHang);
            response.EnsureSuccessStatusCode();
        }
        public async Task<List<KhachHang>> SearchKhachHangAsync(string keyword)
        {
            var result = await _httpClient.GetFromJsonAsync<List<KhachHang>>(
                $"api/KhachHang/search?keyword={Uri.EscapeDataString(keyword)}");
            return result ?? new List<KhachHang>();
        }
    }
}
