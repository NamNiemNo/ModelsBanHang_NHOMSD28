using Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data.Repositories
{
	public class KhachHangRepository : IKhachHangRepository
	{
		private readonly DbContextApp _db;

		public KhachHangRepository(DbContextApp db)
		{
			_db = db;
		}

		public async Task<List<KhachHang>> GetAllAsync()
		{
			return await _db.KhachHangs
				.Include(kh => kh.TaiKhoan)
				.Include(kh => kh.HoaDons)
				.Include(kh => kh.DiaChiKhachHangs)
				.Include(kh => kh.GioHangs)
				.ToListAsync();
		}
        


        public async Task AddAsync(KhachHang khachhang)
		{
			try
			{
                if (khachhang.TaiKhoanId == Guid.Empty)
                {
					khachhang.TaiKhoanId = null;
                }
                await _db.KhachHangs.AddAsync(khachhang);
				await _db.SaveChangesAsync();
			}
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tạo khách hàng: {ex.Message}");
            }
        }

		public async Task UpdateAsync(KhachHang khachhang)
		{
            try
            {
                var existing = await _db.KhachHangs.FindAsync(khachhang.KhachHangId);
                if (existing == null)
                    throw new Exception("Không tìm thấy nhân viên cần cập nhật.");

                // Chỉ cập nhật các trường thay đổi
                _db.Entry(existing).CurrentValues.SetValues(khachhang);

                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi cập nhật nhân viên: " + ex.Message);
            }
        }

		public async Task DeleteAsync(Guid id)
		{
			try
			{
				var entity = await _db.KhachHangs.FindAsync(id);
				if (entity != null)
				{
					_db.KhachHangs.Remove(entity);
					await _db.SaveChangesAsync();
				}
			}
			catch (Exception)
			{
				throw;
			}
		}
        public async Task<List<KhachHang>> SearchKhachHangAsync(string keyword)
        {
            keyword = keyword.ToLower();
            return await _db.KhachHangs
                .Where(nv => nv.HoTen.ToLower().Contains(keyword))
                .ToListAsync();
        }

        public async Task<KhachHang> GetByIdAsync(Guid id)
        {
            try
            {
                return await _db.KhachHangs.FindAsync(id);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}