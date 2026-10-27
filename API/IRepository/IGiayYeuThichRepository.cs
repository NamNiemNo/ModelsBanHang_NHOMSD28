using API.Models;

namespace API.IRepository
{
    public interface IGiayYeuThichRepository
    {
        Task<IEnumerable<GiayYeuThich>> GetAllAsync();
        Task<IEnumerable<GiayYeuThich>> GetByKhachHangIdAsync(Guid khachHangId);
        Task<GiayYeuThich> GetByIdAsync(Guid id);
        Task AddAsync(GiayYeuThich item);
        Task DeleteAsync(Guid id);
        Task<GiayYeuThich> GetByGiayAndKhachHangAsync(Guid giayId, Guid khachHangId);
    }
}
