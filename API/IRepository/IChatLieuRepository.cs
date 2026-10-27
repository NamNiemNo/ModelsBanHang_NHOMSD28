using Data.Models;

namespace API.IRepository
{
    public interface IChatLieuRepository
    {
        Task<IEnumerable<ChatLieu>> GetAllAsync();
        Task<ChatLieu?> GetByIdAsync(Guid id);
        Task AddAsync(ChatLieu entity);
        Task UpdateAsync(ChatLieu entity);
        Task DeleteAsync(Guid id);
    }
}
