using Data.Models;

namespace BlazorKhachHang.Service.IService
{
    public interface IChatLieuService
    {
        Task<List<ChatLieu>> GetAllAsync();
        Task<ChatLieu> GetByIdAsync(Guid id);
        Task CreateAsync(ChatLieu obj);
        Task UpdateAsync(ChatLieu obj);
        Task DeleteAsync(Guid id);
    }
}
