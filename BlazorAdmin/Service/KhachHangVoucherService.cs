using BlazorAdmin.Service.IService;
using Data.Models;
using System.Net.Http.Json;

namespace BlazorAdmin.Service
{
    public class KhachHangVoucherService : IKhachHangVoucherService
    {
        private readonly HttpClient _httpClient;

        public KhachHangVoucherService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<KhachHangVoucher>> GetAllAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<KhachHangVoucher>>("api/KhachHangVoucher")
                ?? new List<KhachHangVoucher>();
        }

        public async Task<List<KhachHangVoucher>> GetByVoucherIdAsync(Guid voucherId)
        {
            return await _httpClient.GetFromJsonAsync<List<KhachHangVoucher>>($"api/KhachHangVoucher/by-voucher/{voucherId}")
                ?? new List<KhachHangVoucher>();
        }

        public async Task<List<KhachHangVoucher>> GetByKhachHangIdAsync(Guid khachHangId)
        {
            return await _httpClient.GetFromJsonAsync<List<KhachHangVoucher>>($"api/KhachHangVoucher/by-khachhang/{khachHangId}")
                ?? new List<KhachHangVoucher>();
        }

        public async Task<bool> AssignOneAsync(KhachHangVoucher khv)
        {
            var response = await _httpClient.PostAsJsonAsync("api/KhachHangVoucher/assign-one", khv);
            if (!response.IsSuccessStatusCode)
            {
                var msg = await response.Content.ReadAsStringAsync();
                Console.WriteLine("⚠️ AssignOneAsync Error: " + msg);
            }
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> AssignMultipleAsync(Guid voucherId, List<Guid> khachHangIds)
        {
            var url = $"api/KhachHangVoucher/assign-multiple?voucherId={voucherId}";
            var response = await _httpClient.PostAsJsonAsync(url, khachHangIds);
            if (!response.IsSuccessStatusCode)
            {
                var msg = await response.Content.ReadAsStringAsync();
                Console.WriteLine("⚠️ AssignMultipleAsync Error: " + msg);
            }
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"api/KhachHangVoucher/{id}");
            if (!response.IsSuccessStatusCode)
            {
                var msg = await response.Content.ReadAsStringAsync();
                Console.WriteLine("⚠️ DeleteAsync Error: " + msg);
            }
            return response.IsSuccessStatusCode;
        }
    }
}
