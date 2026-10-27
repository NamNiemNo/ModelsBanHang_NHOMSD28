using API.Models.DTO;
using BlazorKhachHang.Service.IService;
using System.Net.Http.Json;

namespace BlazorKhachHang.Service
{
    public class GiayChiTietService : IGiayChiTietService
    {
        private readonly HttpClient _httpClient;

        public GiayChiTietService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<List<GiayChiTietDTO>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync("api/GiayChiTiet");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<GiayChiTietDTO>>() ?? new List<GiayChiTietDTO>();
        }

        public async Task<GiayChiTietDTO> GetByIdAsync(Guid id)
        {
            var response = await _httpClient.GetAsync($"api/GiayChiTiet/{id}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<GiayChiTietDTO>() ?? new GiayChiTietDTO();
        }

        public async Task<List<GiayChiTietDTO>> GetByGiayIdAsync(Guid giayId)
        {
            var response = await _httpClient.GetAsync($"api/GiayChiTiet/giay/{giayId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<GiayChiTietDTO>>() ?? new List<GiayChiTietDTO>();
        }

        public async Task<GiayChiTietDTO> CreateAsync(GiayChiTietDTO obj)
        {
            var response = await _httpClient.PostAsJsonAsync("api/GiayChiTiet", obj);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<GiayChiTietDTO>() ?? new GiayChiTietDTO();
        }

        public async Task<List<GiayChiTietDTO>> CreateMultipleAsync(List<GiayChiTietDTO> list)
        {
            var response = await _httpClient.PostAsJsonAsync("api/GiayChiTiet/multiple", list);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<GiayChiTietDTO>>() ?? new List<GiayChiTietDTO>();
        }

        public async Task<GiayChiTietDTO> UpdateAsync(Guid id, GiayChiTietDTO obj)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/GiayChiTiet/{id}", obj);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<GiayChiTietDTO>() ?? new GiayChiTietDTO();
        }
    }
}