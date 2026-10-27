using Data.Models;

namespace API.IRepository
{
    public interface IVoucherRepo
    {
        Task<List<Voucher>> GetAll();
        Task<Voucher> GetById(Guid id);
        Task Create(Voucher voucher);
        Task Update(Voucher voucher);
        Task<bool> Delete(Guid id);
        // thêm
        Task<Voucher?> GetByCode(string code);
    }
}
