using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace API.IRepository.Repository
{
    public class ThuongHieuRepository : IThuongHieuRepository
    {
        private readonly DbContextApp _context;

        public ThuongHieuRepository(DbContextApp context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ThuongHieu>> GetAllAsync()
        {
            return await _context.ThuongHieus.ToListAsync();
        }

        public async Task<ThuongHieu?> GetByIdAsync(Guid id)
        {
            return await _context.ThuongHieus.FindAsync(id);
        }

        public async Task AddAsync(ThuongHieu entity)
        {
            await _context.ThuongHieus.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ThuongHieu entity)
        {
            _context.ThuongHieus.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _context.ThuongHieus.FindAsync(id);
            if (entity != null)
            {
                _context.ThuongHieus.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
