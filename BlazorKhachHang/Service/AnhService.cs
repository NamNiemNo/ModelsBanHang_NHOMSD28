
using API.Models.DTO;
using BlazorKhachHang.Service.IService;
using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace BlazorKhachHang.Service
{
    public class AnhService : IAnhService
    {
        private readonly HttpClient _httpClient;

        public AnhService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }


        public async Task<List<AnhDTO>> GetAllAsync() =>
            await _httpClient.GetFromJsonAsync<List<AnhDTO>>("api/AnhApi");

        public async Task<AnhDTO> GetByIdAsync(Guid id) =>
            await _httpClient.GetFromJsonAsync<AnhDTO>($"api/AnhApi/{id}");

        public async Task<AnhDTO> UploadAsync(IBrowserFile file, string tenAnh, Guid giayChiTietId)
        {
            var content = new MultipartFormDataContent();

            var stream = file.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024);
            var streamContent = new StreamContent(stream);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
            content.Add(streamContent, "file", file.Name);

            content.Add(new StringContent(tenAnh ?? ""), "tenAnh");
            content.Add(new StringContent(giayChiTietId.ToString()), "giayChiTietId");

            var response = await _httpClient.PostAsync("api/AnhApi/upload", content);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<AnhDTO>();
        }

        public async Task<AnhDTO> UpdateAsync(AnhDTO dto)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/AnhApi/{dto.AnhId}", dto);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<AnhDTO>();
        }

        public async Task<AnhDTO> UpdateFileAsync(Guid id, IBrowserFile file, string tenAnh, Guid giayChiTietId)
        {
            var content = new MultipartFormDataContent();

            var stream = file.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024);
            var streamContent = new StreamContent(stream);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
            content.Add(streamContent, "file", file.Name);

            content.Add(new StringContent(tenAnh ?? ""), "tenAnh");
            content.Add(new StringContent(giayChiTietId.ToString()), "giayChiTietId");

            var response = await _httpClient.PutAsync($"api/AnhApi/update-file/{id}", content);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<AnhDTO>();
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"api/AnhApi/{id}");
            response.EnsureSuccessStatusCode();
            return true;
        }
    }
}
