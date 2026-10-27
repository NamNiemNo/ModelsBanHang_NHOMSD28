using API.Models.DTO;
using BlazorKhachHang.Service.IService;
using System.Net.Http.Json;

namespace BlazorKhachHang.Service
{
    public class GiayYeuThichService : IGiayYeuThichService
    {
        private readonly HttpClient _httpClient;

        public GiayYeuThichService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<GiayYeuThichDTO>> GetAllByKhachHangAsync(Guid khachHangId)
        {
            var response = await _httpClient.GetAsync($"api/GiayYeuThich/khachhang/{khachHangId}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<IEnumerable<GiayYeuThichDTO>>();
            }

            return new List<GiayYeuThichDTO>();
        }

        public async Task ToggleFavoriteAsync(Guid giayId, Guid khachHangId)
        {
            var dto = new GiayYeuThichDTO
            {
                GiayId = giayId,
                KhachHangId = khachHangId
            };

            var response = await _httpClient.PostAsJsonAsync("api/GiayYeuThich/toggle", dto);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[ERROR] ToggleFavorite failed: {errorContent}");
                throw new Exception("Không thể thêm vào giày yêu thích");
            }
        }

    }
}
