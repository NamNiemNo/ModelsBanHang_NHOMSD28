
using Data.Models;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace API.IRepository
{
    public interface ITaiKhoanRepository
    {
        Task<List<TaiKhoan>> GetAllTaiKhoanAsync();
        Task<TaiKhoan> GetByIdTaiKhoanAsync(Guid id);
        Task CreateTaiKhoanAsync (TaiKhoan tk);
        Task UpdateTaiKhoanAsync(TaiKhoan tk);
        Task DeleteTaiKhoanAsync(Guid id);
        Task<TaiKhoan> GetByIdChucVuAsync (string username, string password);
        Task<TaiKhoan> GetKhachHang(string username, string password);
        Task<TaiKhoan> GetByUsernameAsync(string username);
        //hahahahah 
    }
}
