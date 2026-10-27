using Data.Models;

namespace API.IRepository
{
    public interface  IAnhRepository
    {
        Task<IEnumerable<Anh>> GetAllAsync();
        Task<Anh?> GetByIdAsync(Guid id);
        Task<Anh?> UploadAsync(IFormFile file, string tenAnh, Guid giayChiTietId);
        Task<Anh?> UpdateAsync(Anh anh);
        Task<Anh?> UpdateFileAsync(Guid id, IFormFile file, string tenAnh, Guid giayChiTietId);
        Task<bool> DeleteAsync(Guid id);
    }
}
