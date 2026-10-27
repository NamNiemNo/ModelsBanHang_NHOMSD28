using API.Models.DTO;

namespace BlazorAdmin.Service.IService
{
    public interface IThongBaoService
    {
        Task<List<ThongBaoDTO>> GetThongBaoMoiAsync();
    }
}
