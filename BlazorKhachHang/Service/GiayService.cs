using API.Models.DTO;
using BlazorKhachHang.Service.IService;
using System.Net.Http.Json;

namespace BlazorKhachHang.Service
{
    public class GiayService : IGiayService
    {
        private readonly HttpClient _httpClient;

        public GiayService(
            HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<GiayDTO>>
            GetAllAsync()
        {
            return await _httpClient
                       .GetFromJsonAsync<List<GiayDTO>>(
                           "api/Giay")
                   ?? new List<GiayDTO>();
        }

        public async Task<GiayDTO?>
            GetByIdAsync(Guid id)
        {
            return await _httpClient
                .GetFromJsonAsync<GiayDTO>(
                    $"api/Giay/{id}");
        }

        public async Task<GiayDTO?>
            CreateAsync(GiayDTO obj)
        {
            var response =
                await _httpClient.PostAsJsonAsync(
                    "api/Giay",
                    obj);

            response.EnsureSuccessStatusCode();

            return await response.Content
                .ReadFromJsonAsync<GiayDTO>();
        }

        public async Task UpdateAsync(
            GiayDTO obj)
        {
            var response =
                await _httpClient.PutAsJsonAsync(
                    $"api/Giay/{obj.GiayId}",
                    obj);

            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(Guid id)
        {
            var response =
                await _httpClient.DeleteAsync(
                    $"api/Giay/{id}");

            response.EnsureSuccessStatusCode();
        }

        public async Task<List<GiayDTO>>
            SearchAsync(string keyword)
        {
            var encodedKeyword =
                Uri.EscapeDataString(keyword);

            return await _httpClient
                       .GetFromJsonAsync<List<GiayDTO>>(
                           $"api/Giay/search?keyword={encodedKeyword}")
                   ?? new List<GiayDTO>();
        }
    }
}