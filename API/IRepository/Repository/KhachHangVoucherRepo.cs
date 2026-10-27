using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace API.IRepository.Repository
{
	public class KhachHangVoucherRepo : IKhachHangVoucherRepo
	{
		private readonly DbContextApp _context;

		public KhachHangVoucherRepo(DbContextApp context)
		{
			_context = context;
		}

		public async Task<List<KhachHangVoucher>> GetAllAsync()
		{
			return await _context.KhachHangVouchers
				.Include(x => x.KhachHang)
				.Include(x => x.Voucher)
				.AsNoTracking()
				.ToListAsync();
		}

		public async Task<List<KhachHangVoucher>> GetByVoucherIdAsync(Guid voucherId)
		{
			return await _context.KhachHangVouchers
				.Where(x => x.VoucherId == voucherId)
				.Include(x => x.KhachHang)
				.Include(x => x.Voucher)
				.AsNoTracking()
				.ToListAsync();
		}

		public async Task<List<KhachHangVoucher>> GetByKhachHangIdAsync(Guid khachHangId)
		{
			return await _context.KhachHangVouchers
				.Where(x => x.KhachHangId == khachHangId)
				.Include(x => x.Voucher)
				.AsNoTracking()
				.ToListAsync();
		}

		public async Task<bool> AddAsync(KhachHangVoucher entity)
		{
			var exists = await _context.KhachHangVouchers
				.AnyAsync(x => x.VoucherId == entity.VoucherId && x.KhachHangId == entity.KhachHangId);

			if (exists)
				return false;

			entity.Id = Guid.NewGuid();
			entity.NgayTao = DateTime.Now;
			entity.DaSuDung = false;

			_context.KhachHangVouchers.Add(entity);
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task AddMultipleAsync(Guid voucherId, List<Guid> khachHangIds)
		{
			var existingPairs = await _context.KhachHangVouchers
				.Where(x => x.VoucherId == voucherId && khachHangIds.Contains(x.KhachHangId))
				.Select(x => x.KhachHangId)
				.ToListAsync();

			var newKhachHangIds = khachHangIds.Except(existingPairs);

			foreach (var khId in newKhachHangIds)
			{
				_context.KhachHangVouchers.Add(new KhachHangVoucher
				{
					Id = Guid.NewGuid(),
					VoucherId = voucherId,
					KhachHangId = khId,
					NgayTao = DateTime.Now,
					DaSuDung = false
				});
			}

			await _context.SaveChangesAsync();
		}

		public async Task<bool> DeleteAsync(Guid id)
		{
			var entity = await _context.KhachHangVouchers.FindAsync(id);
			if (entity == null)
				return false;

			_context.KhachHangVouchers.Remove(entity);
			await _context.SaveChangesAsync();
			return true;
		}
        public async Task<Voucher?> GetVoucherByIdAsync(Guid voucherId)
        {
            return await _context.Vouchers.FindAsync(voucherId);
        }



    }
}
