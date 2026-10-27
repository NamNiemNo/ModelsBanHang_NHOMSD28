
using Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Security;

namespace API.IRepository.Repository
{

    public class NhanVienRepository : INhanVienRepository
    {
        private readonly DbContextApp _db;
        public NhanVienRepository(DbContextApp db)
        {
            _db = db;
        }
        public async Task CreateNhanVien(NhanVien nhanVien)
        {
            try
            {

                if (nhanVien.TaikhoanId == Guid.Empty)
                {
                    nhanVien.TaikhoanId = null;
                }


                if (nhanVien.ChucVuId == null || nhanVien.ChucVuId == Guid.Empty)
                {
                    nhanVien.ChucVuId = Guid.Parse("22222222-2222-2222-2222-222222222222");
                }


                nhanVien.NgayCapNhatCuoiCung = DateTime.Now;

                _db.NhanViens.Add(nhanVien);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tạo nhân viên: {ex.Message}");
            }
        }

        public async Task DeleteNhanVienAsync(Guid NhanVienId)
        {
            try
            {
                var FindIdNhanVien = await GetByIdNhanVienAsync(NhanVienId);
                if (FindIdNhanVien != null)
                {
                    _db.Remove<NhanVien>(FindIdNhanVien);
                    await _db.SaveChangesAsync();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<NhanVien>> GetAllNhanVienAsync()
        {
            return await _db.NhanViens
                            .Include(nv => nv.ChucVu)
                            .Include(nv => nv.TaiKhoan)
                            .ToListAsync();
        }

        public async Task<NhanVien> GetByIdNhanVienAsync(Guid NhanVienId)
        {
            try
            {
                return await _db.NhanViens
                 .Include(nv => nv.TaiKhoan)
                 .Include(nv => nv.ChucVu)
                 .FirstOrDefaultAsync(nv => nv.NhanVienId == NhanVienId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<NhanVien> GetIdNhanVienTaiKhoan(Guid tk)
        {
            return await _db.NhanViens.FirstOrDefaultAsync(TK => TK.TaikhoanId == tk);
        }

        public async Task UpdateNhanVienAsync(NhanVien nhanVien)
        {
            try
            {
                var existing = await _db.NhanViens.FindAsync(nhanVien.NhanVienId);
                if (existing == null)
                    throw new Exception("Không tìm thấy nhân viên cần cập nhật.");

                // Chỉ cập nhật các trường thay đổi
                _db.Entry(existing).CurrentValues.SetValues(nhanVien);

                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi cập nhật nhân viên: " + ex.Message);
            }
        }

        public async Task<List<NhanVien>> SearchNhanVienAsync(string keyword)
        {
            keyword = keyword.ToLower();
            return await _db.NhanViens
                .Where(nv => nv.HoTen.ToLower().Contains(keyword))
                .ToListAsync();
        }


    }
}
