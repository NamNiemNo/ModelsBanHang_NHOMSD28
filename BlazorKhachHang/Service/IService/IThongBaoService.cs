using API.Models.DTO;

namespace BlazorKhachHang.Service.IService
{
    public interface IThongBaoService
    {
        Task<List<ThongBaoDTO>> GetThongBaoMoiAsync();
    }
}
