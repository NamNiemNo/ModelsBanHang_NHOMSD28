using Data.Models;

namespace BlazorKhachHang.Service.IService
{
    public interface IGiamGiaService
    {
        Task<IEnumerable<GiamGia>> GetAllAsync();
        Task<GiamGia> GetByIdAsync(Guid id);
        Task<HttpResponseMessage> AddAsync(GiamGia giamGia); // <== Sửa dòng này
        Task<HttpResponseMessage> UpdateAsyncReturnResponse(GiamGia giamGia);

        Task<bool> DeleteAsync(Guid id);
        Task<bool> AddGiayToDotGiamGia(Guid giamGiaId, Guid giayId);
        Task<bool> RemoveGiayFromDotGiamGia(Guid giamGiaId, Guid giayId);
    }

}