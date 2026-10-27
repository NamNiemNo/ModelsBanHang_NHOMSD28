using API.Models.DTO.BanHang;

namespace BlazorKhachHang.Service.IService
{
    public interface IXuLyDiaChi
    {
        Task<List<DiaChi>> ParseDiaChiAsync(string filePath);
    }
}
