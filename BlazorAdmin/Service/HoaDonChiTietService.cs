using BlazorAdmin.Service.IService;
using Data.Models;
using System.Text.Json;

namespace BlazorAdmin.Service
{
    public class HoaDonChiTietService : IHoaDonChiTietService
    {
        private readonly HttpClient _httpClient;

        public HoaDonChiTietService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<HoaDonChiTiet>> GetAllAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<HoaDonChiTiet>>("api/HoaDonChiTiets");
        }
        public async Task<HoaDonChiTiet> Create(HoaDonChiTiet hoaDon)
        {
            try 
            {
                var response = await _httpClient.PostAsJsonAsync("api/HoaDonChiTiets", hoaDon);
                var content = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    return JsonSerializer.Deserialize<HoaDonChiTiet>(content, new JsonSerializerOptions
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
    }
}
