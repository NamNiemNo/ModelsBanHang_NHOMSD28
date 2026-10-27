using BlazorKhachHang.Service.IService;
using Data.Models;

namespace BlazorKhachHang.Service
{
    public class ChatLieuService : IChatLieuService
    {
        private readonly HttpClient _httpClient;

        public ChatLieuService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ChatLieu>> GetAllAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<ChatLieu>>("api/ChatLieu");
        }

        public async Task<ChatLieu> GetByIdAsync(Guid id)
        {
            return await _httpClient.GetFromJsonAsync<ChatLieu>($"api/ChatLieu/{id}");
        }

        public async Task CreateAsync(ChatLieu obj)
        {
            await _httpClient.PostAsJsonAsync("api/ChatLieu", obj);
        }

        public async Task UpdateAsync(ChatLieu obj)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/ChatLieu/{obj.ChatLieuId}", obj);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(Guid id)
        {
            await _httpClient.DeleteAsync($"api/ChatLieu/{id}");
        }
    }
}
