using Data.Models;

namespace API.IRepository
{
	public interface IKhachHangVoucherRepo
	{
		Task<List<KhachHangVoucher>> GetAllAsync();
		Task<List<KhachHangVoucher>> GetByVoucherIdAsync(Guid voucherId);
		Task<List<KhachHangVoucher>> GetByKhachHangIdAsync(Guid khachHangId);
		Task<bool> AddAsync(KhachHangVoucher entity);
        Task<Voucher?> GetVoucherByIdAsync(Guid voucherId);

        Task AddMultipleAsync(Guid voucherId, List<Guid> khachHangIds);
		Task<bool> DeleteAsync(Guid id);

      
    }
}
