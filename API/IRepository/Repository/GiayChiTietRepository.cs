using API.IRepository;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Repository
{
    public class GiayChiTietRepository : IGiayChiTietRepository
    {
        private readonly DbContextApp _context;

        public GiayChiTietRepository(DbContextApp context)
        {
            _context = context;
        }

        public async Task<IEnumerable<GiayChiTiet>> GetAllAsync()
        {
            return await _context.GiayChiTiets
                .Include(g => g.Giay)
                .Include(g => g.KichCo)
                .Include(g => g.MauSac)
                .Include(g => g.Anhs)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<GiayChiTiet?> GetByIdAsync(Guid id)
        {
            return await _context.GiayChiTiets
                .AsNoTracking() // Thêm vào đây
                .Include(g => g.Giay)
                .Include(g => g.KichCo)
                .Include(g => g.MauSac)
                .Include(g => g.Anhs)
                .FirstOrDefaultAsync(x => x.GiayChiTietId == id);
        }

        public async Task<IEnumerable<GiayChiTiet>> GetByGiayIdAsync(Guid giayId)
        {
            return await _context.GiayChiTiets
                .Where(x => x.GiayId == giayId)
                .Include(g => g.Giay)
                .Include(g => g.KichCo)
                .Include(g => g.MauSac)
                .Include(g => g.Anhs)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<GiayChiTiet> AddAsync(GiayChiTiet chiTiet)
        {
            if (await _context.GiayChiTiets.AnyAsync(x =>
                x.GiayId == chiTiet.GiayId &&
                x.KichCoId == chiTiet.KichCoId &&
                x.MauSacId == chiTiet.MauSacId))
            {
                throw new InvalidOperationException("Đã tồn tại sản phẩm với cùng cấu hình");
            }

            chiTiet.GiayChiTietId = Guid.NewGuid();
            chiTiet.NgayTao = DateTime.UtcNow;
            chiTiet.NgaySua = DateTime.UtcNow;

            await _context.GiayChiTiets.AddAsync(chiTiet);
            await _context.SaveChangesAsync();
            return chiTiet;
        }

        public async Task<List<GiayChiTiet>> AddMultipleReturnAsync(List<GiayChiTiet> list)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var item in list)
                {
                    if (await _context.GiayChiTiets.AnyAsync(x =>
                        x.GiayId == item.GiayId &&
                        x.KichCoId == item.KichCoId &&
                        x.MauSacId == item.MauSacId))
                    {
                        throw new InvalidOperationException($"Đã tồn tại sản phẩm với cấu hình: GiayId={item.GiayId}, KichCoId={item.KichCoId}, MauSacId={item.MauSacId}");
                    }

                    item.GiayChiTietId = Guid.NewGuid();
                    item.NgayTao = DateTime.UtcNow;
                    item.NgaySua = DateTime.UtcNow;
                    await _context.GiayChiTiets.AddAsync(item);
                }
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return list;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<GiayChiTiet?> UpdateAsync(GiayChiTiet chiTiet)
        {
            var existing = await _context.GiayChiTiets.FindAsync(chiTiet.GiayChiTietId);
            if (existing == null) return null;

            existing.GiayId = chiTiet.GiayId;
            existing.KichCoId = chiTiet.KichCoId;
            existing.MauSacId = chiTiet.MauSacId;
            existing.Gia = chiTiet.Gia;
            existing.SoLuongCon = chiTiet.SoLuongCon;
            existing.MoTa = chiTiet.MoTa;
            existing.TrangThai = chiTiet.TrangThai;
            existing.NgaySua = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existing;
        }
    }
}