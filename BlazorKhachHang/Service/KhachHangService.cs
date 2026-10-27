using API.Models.DTO.BanHang;
using Application.DTOs;
using BlazorKhachHang.Components.Pages;
using BlazorKhachHang.Service.IService;
using Data.Models;

namespace BlazorKhachHang.Service
{
    // hahaha
    public class KhachHangService : IKhachHangService
    {
        private readonly HttpClient _httpClient;

        public KhachHangService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task Create(KhachHang khachHang)
        {
            await _httpClient.PostAsJsonAsync("api/KhachHang", khachHang);
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
        // thêm địa chỉ
        public async Task AddAddress(DiaChiKhachHangDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/DiaChiKhachHang", dto);
            response.EnsureSuccessStatusCode();
        }
        // lấy địa chỉ khách hàng 
        public async Task<List<DiaChiKhachHangDto>> GetAddress(Guid khachHangId)
        {
            return await _httpClient.GetFromJsonAsync<List<DiaChiKhachHangDto>>
                ($"api/DiaChiKhachHang/khachhang/{khachHangId}") ?? new();
        }
        // update profile 
        public async Task UpdateProfile(Guid id, KhachHangUpdateDTO dto)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/KhachHang/update-profile/{id}", dto);
            response.EnsureSuccessStatusCode();
        }
        
    }
}
