using System.Net.Http.Json;
using API.Models.DTO;
using BlazorKhachHang.Service.IService;

public class KieuDangService : IKieuDangService
{
    private readonly HttpClient _http;

    public KieuDangService(HttpClient http)
    {
        _http = http;
    }

    public async Task<IEnumerable<KieuDangDTO>> GetAllAsync()
    {
        return await _http.GetFromJsonAsync<IEnumerable<KieuDangDTO>>("api/KieuDang");
    }

    public async Task<KieuDangDTO?> GetByIdAsync(Guid id)
    {
        return await _http.GetFromJsonAsync<KieuDangDTO>($"api/KieuDang/{id}");
    }

    public async Task<IEnumerable<KieuDangDTO>> SearchByNameAsync(string keyword)
    {
        return await _http.GetFromJsonAsync<IEnumerable<KieuDangDTO>>($"api/KieuDang/search?keyword={keyword}");
    }

    public async Task<KieuDangDTO?> CreateAsync(KieuDangDTO dto)
    {
        var response = await _http.PostAsJsonAsync("api/KieuDang", dto);
        return await response.Content.ReadFromJsonAsync<KieuDangDTO>();
    }

    public async Task<KieuDangDTO?> UpdateAsync(Guid id, KieuDangDTO dto)
    {
        var response = await _http.PutAsJsonAsync($"api/KieuDang/{id}", dto);
        return response.IsSuccessStatusCode ? dto : null;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var response = await _http.DeleteAsync($"api/KieuDang/{id}");
        return response.IsSuccessStatusCode;
    }
}
