using API.Models.DTO;
using BlazorAdmin.Service.IService;
using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace BlazorAdmin.Service
{
    public class AnhService : IAnhService
    {
        private readonly HttpClient _httpClient;
        private const int MaxFileSize = 10 * 1024 * 1024; // 10MB

        public AnhService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<AnhDTO>> GetAllAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<AnhDTO>>("api/AnhApi") ?? new List<AnhDTO>();
        }

        public async Task<AnhDTO?> GetByIdAsync(Guid id)
        {
            return await _httpClient.GetFromJsonAsync<AnhDTO>($"api/AnhApi/{id}");
        }

        public async Task<AnhDTO?> UploadAsync(IBrowserFile file, string tenAnh, Guid giayChiTietId)
        {
            try
            {
                using var content = new MultipartFormDataContent();
                await AddFileToContent(file, content);
                content.Add(new StringContent(tenAnh), "tenAnh");
                content.Add(new StringContent(giayChiTietId.ToString()), "giayChiTietId");

                var response = await _httpClient.PostAsync("api/AnhApi/upload", content);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<AnhDTO>();
            }
            catch
            {
                return null;
            }
        }

        public async Task<AnhDTO?> UpdateAsync(AnhDTO dto)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/AnhApi/{dto.AnhId}", dto);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<AnhDTO>();
            }
            catch
            {
                return null;
            }
        }

        public async Task<AnhDTO?> UpdateFileAsync(Guid id, IBrowserFile file, string tenAnh, Guid giayChiTietId)
        {
            try
            {
                using var content = new MultipartFormDataContent();
                await AddFileToContent(file, content);
                content.Add(new StringContent(tenAnh), "tenAnh");
                content.Add(new StringContent(giayChiTietId.ToString()), "giayChiTietId");

                var response = await _httpClient.PutAsync($"api/AnhApi/update-file/{id}", content);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<AnhDTO>();
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/AnhApi/{id}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        private async Task AddFileToContent(IBrowserFile file, MultipartFormDataContent content)
        {
            var stream = file.OpenReadStream(MaxFileSize);
            var streamContent = new StreamContent(stream);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
            content.Add(streamContent, "file", file.Name);
        }
    }
}