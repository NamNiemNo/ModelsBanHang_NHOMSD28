using API.Models.DTO;

namespace BlazorKhachHang.Service.IService
{
    public interface IGiayYeuThichService
    {
        Task<IEnumerable<GiayYeuThichDTO>> GetAllByKhachHangAsync(Guid khachHangId);
        Task ToggleFavoriteAsync(Guid giayId, Guid khachHangId);
    }
}
