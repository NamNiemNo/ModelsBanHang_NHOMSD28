using BlazorKhachHang.Service.IService;
using Data.Models;
using System.Net.Http.Json;

namespace BlazorKhachHang.Service
{
    public class ChiTietHoaDonService : IChiTietHoaDonService
    {
        private readonly HttpClient _httpClient;

        public ChiTietHoaDonService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<HoaDonChiTiet>> GetAllHDCTAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<HoaDonChiTiet>>("/api/HoaDonChiTiets");
        }

        public async Task<HoaDonChiTiet> GetByHoaDonChiTietIdAsync(Guid hdctID)
        {
            return await _httpClient.GetFromJsonAsync<HoaDonChiTiet>($"/api/HoaDonChiTiets/{hdctID}");
        }

        public async Task<HoaDonChiTiet> GetByHoaDonChiTietvaGiayAsync(Guid hoaDonId, Guid giayId)
        {
            return await _httpClient.GetFromJsonAsync<HoaDonChiTiet>($"/api/HoaDonChiTiets/{hoaDonId}/{giayId}");
        }

        public async Task<HoaDonChiTiet> GetByIdHDCTAsync(Guid idCTHD)
        {
            return await _httpClient.GetFromJsonAsync<HoaDonChiTiet>($"/api/HoaDonChiTiets/id/{idCTHD}");
        }
    }
}
