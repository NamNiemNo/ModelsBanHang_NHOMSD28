using API.Models.DTO;
using Application.DTOs;
using BlazorKhachHang.Service.IService;

namespace BlazorKhachHang.Service
{
    public class DiaChiKhachHangService : IDiaChiKhachHangService
    {
        private readonly HttpClient _httpClient;

        public DiaChiKhachHangService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<DiaChiKhachHangDto>> GetAllAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<DiaChiKhachHangDto>>("api/DiaChiKhachHang");
        }

        public async Task<DiaChiKhachHangDto> GetByIdAsync(Guid id)
        {
            return await _httpClient.GetFromJsonAsync<DiaChiKhachHangDto>($"api/DiaChiKhachHang/{id}");
        }

        public async Task<List<DiaChiKhachHangDto>> GetByKhachHangIdAsync(Guid khachHangId)
        {
            return await _httpClient.GetFromJsonAsync<List<DiaChiKhachHangDto>>($"api/DiaChiKhachHang/khachhang/{khachHangId}");
        }

        public async Task<DiaChiKhachHangDto> GetDefaultByKhachHangIdAsync(Guid khachHangId)
        {
            return await _httpClient.GetFromJsonAsync<DiaChiKhachHangDto>($"api/DiaChiKhachHang/default/{khachHangId}");
        }

        public async Task<bool> CreateAsync(DiaChiKhachHangDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/DiaChiKhachHang", dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsync(Guid id, DiaChiKhachHangDto dto)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/DiaChiKhachHang/{id}", dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"api/DiaChiKhachHang/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> SetDefaultAsync(Guid id)
        {
            var response = await _httpClient.PutAsync($"api/DiaChiKhachHang/set-default/{id}", null);
            return response.IsSuccessStatusCode;
        }
    }
}
