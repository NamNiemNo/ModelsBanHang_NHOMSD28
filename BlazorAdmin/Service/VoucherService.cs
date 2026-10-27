using BlazorAdmin.Service.IService;
using Data.Models;
using System.Net.Http.Json;

namespace BlazorAdmin.Service
{
    public class VoucherService : IVoucherService
    {
        private readonly HttpClient _httpClient;

        public VoucherService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Voucher>> GetAll()
        {
            return await _httpClient.GetFromJsonAsync<List<Voucher>>("/api/Voucher");
        }

        public async Task<Voucher> GetById(Guid id)
        {
            return await _httpClient.GetFromJsonAsync<Voucher>($"/api/Voucher/{id}");
        }

        public async Task Add(Voucher voucher)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/Voucher", voucher);
            if (!response.IsSuccessStatusCode)
            {
                var msg = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Lỗi Add(): " + msg);
            }
        }

        public async Task Update(Voucher voucher)
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/Voucher/{voucher.VoucherId}", voucher);
            if (!response.IsSuccessStatusCode)
            {
                var msg = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Lỗi Update(): " + msg);
            }
        }

        public async Task Delete(Guid id)
        {
            await _httpClient.DeleteAsync($"/api/Voucher/{id}");
        }
    }
}
