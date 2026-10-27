
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace API.IRepository.Repository
{
    public class TaiKhoanRepository : ITaiKhoanRepository
    {
        private readonly DbContextApp _contextApp;
        public TaiKhoanRepository(DbContextApp contextApp)
        {
            _contextApp = contextApp;
        }

        public async Task CreateTaiKhoanAsync(TaiKhoan tk)
        {
            try
            {
                _contextApp.TaiKhoans.Add(tk);
               await _contextApp.SaveChangesAsync();    
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task DeleteTaiKhoanAsync(Guid id)
        {
            try
            {
                var taiKhoan = await _contextApp.TaiKhoans.FindAsync(id);
                if (taiKhoan == null)
                {
                    throw new Exception("Không tìm thấy tài khoản để xóa.");
                }

                _contextApp.TaiKhoans.Remove(taiKhoan);
                await _contextApp.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }


        public Task<List<TaiKhoan>> GetAllTaiKhoanAsync()
        {
           return _contextApp.TaiKhoans.ToListAsync();
        }

        public async Task<TaiKhoan> GetByIdChucVuAsync(string username, string password)
        {
            return await _contextApp.TaiKhoans
                .Include(tk => tk.NhanVien)
                .ThenInclude(nv => nv.ChucVu)
                                    .FirstOrDefaultAsync(x => x.Username == username && x.Password == password);
        }



        public async Task<TaiKhoan> GetByIdTaiKhoanAsync(Guid id)
        {
            try
            {
                return await _contextApp.TaiKhoans.FindAsync(id);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task UpdateTaiKhoanAsync(TaiKhoan tk)
        {
            try
            {
                var existing = await _contextApp.TaiKhoans.FindAsync(tk.TaikhoanId);
                if (existing == null) throw new Exception("Không tìm thấy tài khoản.");

                _contextApp.Entry(existing).CurrentValues.SetValues(tk);

                await _contextApp.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<TaiKhoan> GetByUsernameAsync(string username)
        {
            var tk = await _contextApp.TaiKhoans.FirstOrDefaultAsync(tk => tk.Username == username);
            if (tk == null)
            {
                Console.WriteLine($"Không tìm thấy username: {username}");
            }
            return tk;
        }

        public async Task<TaiKhoan> GetKhachHang(string username, string password)
        {
            return await _contextApp.TaiKhoans
               .Include(tk => tk.KhachHang)
                    .FirstOrDefaultAsync(x => x.Username == username && x.Password == password);
        }
    }
}
