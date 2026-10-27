using Data.Models;

namespace BlazorKhachHang.Service.IService
{
    public interface IVoucherService
    {
        Task<List<Voucher>> GetAll();

        Task<Voucher> GetById(Guid id);

        Task Add(Voucher voucher);

        Task Update(Voucher voucher);

        Task Delete(Guid id);

        Task<Voucher?> GetByCodeAsync(string code);
    }
}
