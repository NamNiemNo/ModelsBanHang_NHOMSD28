
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Models;

namespace API.IRepository
{
    public interface IChucVuRepository
    {
        Task<IEnumerable<ChucVu>> GetAllChucVuAsync();
        Task<ChucVu> GetByIdChucVuAsync(Guid id);
        Task CreateChucVuAsync(ChucVu chucVu);
        Task UpdateChucVuAsync(ChucVu chucVu);
        Task DeleteChucVuAsync(Guid id);
    }
}