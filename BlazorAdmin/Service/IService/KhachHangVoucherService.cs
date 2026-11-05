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
        Task<SendVoucherEmailResult?> SendVoucherEmailAsync(Guid voucherId, bool sendToAll, List<Guid> customerIds);
    }

    public sealed class SendVoucherEmailResult
    {
        public string Message { get; set; } = string.Empty;
        public int SentCount { get; set; }
        public int FailedCount { get; set; }
        public int SkippedCount { get; set; }
        public List<object> FailedCustomers { get; set; } = new();
    }
}
