using API.Models.DTO.BanHang;

namespace BlazorAdmin.Service.IService
{
    public interface IXuLyDiaChi
    {
        Task<List<DiaChi>> ParseDiaChiAsync(string filePath);
    }
}
