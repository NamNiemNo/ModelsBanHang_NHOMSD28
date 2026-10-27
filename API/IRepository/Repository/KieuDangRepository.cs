using Data.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace API.IRepository.Repository
{
    public class KieuDangRepository : IKieuDangRepository
    {
        private readonly DbContextApp _context;

        public KieuDangRepository(DbContextApp context)
        {
            _context = context;
        }

        public async Task<IEnumerable<KieuDang>> GetAllAsync()
        {
            return await _context.KieuDangs.ToListAsync();
        }

        public async Task<KieuDang?> GetByIdAsync(Guid id)
        {
            return await _context.KieuDangs.FindAsync(id);
        }

        public async Task<IEnumerable<KieuDang>> SearchByNameAsync(string keyword)
        {
            return await _context.KieuDangs
                .Where(k => k.TenKieuDang.Contains(keyword))
                .ToListAsync();
        }

        public async Task<KieuDang> AddAsync(KieuDang kieuDang)
        {
            _context.KieuDangs.Add(kieuDang);
            await _context.SaveChangesAsync();
            return kieuDang;
        }

        public async Task<KieuDang> UpdateAsync(KieuDang kieuDang)
        {
            var existing = await _context.KieuDangs.FindAsync(kieuDang.KieuDangId);
            if (existing == null) return null;

            existing.TenKieuDang = kieuDang.TenKieuDang;
            existing.MoTa = kieuDang.MoTa;
            existing.TrangThai = kieuDang.TrangThai;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var kieuDang = await _context.KieuDangs.FindAsync(id);
            if (kieuDang == null) return false;

            _context.KieuDangs.Remove(kieuDang);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
