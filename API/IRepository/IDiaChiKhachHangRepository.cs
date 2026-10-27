using Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data.IRepository
{
    public interface IDiaChiKhachHangRepository
    {
        Task<List<DiaChiKhachHang>> GetAllAsync();
        Task<DiaChiKhachHang> GetByIdAsync(Guid id);
        Task<List<DiaChiKhachHang>> GetByKhachHangIdAsync(Guid khachHangId);
        Task<DiaChiKhachHang> GetDefaultByKhachHangIdAsync(Guid khachHangId);
        Task<bool> CreateAsync(DiaChiKhachHang diaChi);
        Task<bool> UpdateAsync(DiaChiKhachHang diaChi);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> SetDefaultAsync(Guid id);
    }
}
