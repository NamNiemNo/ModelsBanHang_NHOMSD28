using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace API.IRepository.Repository
{
    public class ChatLieuRepository : IChatLieuRepository
    {
        private readonly DbContextApp _context;

        public ChatLieuRepository(DbContextApp context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ChatLieu>> GetAllAsync()
        {
            return await _context.ChatLieus.ToListAsync();
        }

        public async Task<ChatLieu?> GetByIdAsync(Guid id)
        {
            return await _context.ChatLieus.FindAsync(id);
        }

        public async Task AddAsync(ChatLieu entity)
        {
            await _context.ChatLieus.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ChatLieu entity)
        {
            _context.ChatLieus.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _context.ChatLieus.FindAsync(id);
            if (entity != null)
            {
                _context.ChatLieus.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
