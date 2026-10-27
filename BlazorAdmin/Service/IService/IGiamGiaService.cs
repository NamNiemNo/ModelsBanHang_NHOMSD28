using Data.Models;
using API.Models.DTO; // 👈 Thêm namespace để dùng DTO

namespace BlazorAdmin.Service.IService
{
    public interface IGiamGiaService
    {
        Task<IEnumerable<GiamGia>> GetAllAsync();
        Task<GiamGia> GetByIdAsync(Guid id);
        Task<HttpResponseMessage> AddAsync(GiamGiaCreateDTO giamGia);
        Task<HttpResponseMessage> UpdateAsyncReturnResponse(GiamGiaCreateDTO giamGia);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> AddGiayToDotGiamGia(Guid giamGiaId, Guid giayChiTietId);
        Task<bool> RemoveGiayFromDotGiamGia(Guid giamGiaId, Guid giayChiTietId);
    }
}
