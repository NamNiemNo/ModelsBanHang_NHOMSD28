
using API.Models.DTO;
using BlazorKhachHang.Service.IService;


namespace BlazorKhachHang.Service
{
    public class TheLoaiGiayService : ITheLoaiGiayService
    {
        private readonly HttpClient _httpClient;

        public TheLoaiGiayService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<TheLoaiGiayDTO>> GetAllAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<TheLoaiGiayDTO>>("api/TheLoaiGiay");
        }

        public async Task<TheLoaiGiayDTO> GetByIdAsync(Guid id)
        {
            return await _httpClient.GetFromJsonAsync<TheLoaiGiayDTO>($"api/TheLoaiGiay/{id}");
        }

        public async Task<TheLoaiGiayDTO> AddAsync(TheLoaiGiayDTO dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/TheLoaiGiay", dto);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<TheLoaiGiayDTO>();
        }

        public async Task<TheLoaiGiayDTO> UpdateAsync(TheLoaiGiayDTO dto)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/TheLoaiGiay/{dto.TheLoaiGiayId}", dto);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<TheLoaiGiayDTO>();
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"api/TheLoaiGiay/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<IEnumerable<TheLoaiGiayDTO>> SearchByNameAsync(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return Enumerable.Empty<TheLoaiGiayDTO>();

            return await _httpClient.GetFromJsonAsync<IEnumerable<TheLoaiGiayDTO>>(
                $"api/TheLoaiGiay/search?keyword={Uri.EscapeDataString(keyword)}");
        }
    }
}