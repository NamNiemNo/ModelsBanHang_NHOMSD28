using Data.Models;

namespace BlazorAdmin.Service.IService
{
    public interface IVoucherService
    {
        Task<List<Voucher>> GetAll();
        Task<Voucher> GetById(Guid id);
        Task Add(Voucher voucher);
        Task Update(Voucher voucher);
        Task Delete(Guid id);
    }
}
