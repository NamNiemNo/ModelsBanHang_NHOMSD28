
using API.Models.DTO;
using Data.Models;

namespace BlazorKhachHang.Service.IService
{
    public interface ITaiKhoanService
    {
        Task<List<TaiKhoan>> GetAllTaiKhoanAsync();
        Task<TaiKhoan> GetByIdTaiKhoanAsync(Guid id);
        Task CreateTaiKhoanAsync(TaiKhoan tk);
        Task UpdateTaiKhoanAsync(TaiKhoan tk);
        Task DeleteTaiKhoanAsync(Guid id);
        Task<LoginResponseDto> GetKhachHang(string username, string password );
        Task<TaiKhoan> GetByUsernameAsync(string username);

        Task<HttpResponseMessage> RegisterCustomerAsync(RegisterCustomerDTO request);


    }
}
