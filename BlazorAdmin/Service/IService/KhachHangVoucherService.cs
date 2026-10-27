using Data.Models;

namespace BlazorAdmin.Service.IService
{
	public interface IKhachHangVoucherService
	{
		Task<List<KhachHangVoucher>> GetAllAsync();
		Task<List<KhachHangVoucher>> GetByVoucherIdAsync(Guid voucherId);
		Task<List<KhachHangVoucher>> GetByKhachHangIdAsync(Guid khachHangId);
		Task<bool> AssignOneAsync(KhachHangVoucher khv);
		Task<bool> AssignMultipleAsync(Guid voucherId, List<Guid> khachHangIds);
		Task<bool> DeleteAsync(Guid id);
	}
}
