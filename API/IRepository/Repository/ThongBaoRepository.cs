using API.IRepository;
using API.Models;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace API.IRepository.Repository
{
    public class ThongBaoRepository : IThongBaoRepository
    {
        private readonly DbContextApp _context;

        public ThongBaoRepository(DbContextApp context)
        {
            _context = context;
        }

        public async Task<List<ThongBao>> GetThongBaoMoiAsync()
        {
            return await _context.ThongBaos
                .Where(tb => !tb.DaXem)
                .OrderByDescending(tb => tb.ThoiGian)
                .ToListAsync();
        }

        public async Task ThemThongBaoAsync(string noiDung)
        {
            var tb = new ThongBao
            {
                NoiDung = noiDung,
                ThoiGian = DateTime.Now,
                DaXem = false
            };

            _context.ThongBaos.Add(tb);
            await _context.SaveChangesAsync();
        }
    }
}
