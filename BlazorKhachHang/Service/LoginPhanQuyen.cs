using API.Models.DTO;
using BlazorKhachHang.Service.IService;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.JSInterop;

namespace BlazorKhachHang.Service
{
    public class LoginPhanQuyen
    {
        private readonly ITaiKhoanService taiKhoanService;
        private readonly IJSRuntime JS;
        public async Task<bool> PhanQuyen(string username , string password)
        {
            try
            {
                var loginResponseDto = await taiKhoanService.GetKhachHang(username, password);

                if (loginResponseDto.IsSuccess)
                {

                    await JS.InvokeVoidAsync("localStorage.setItem", "currentId", loginResponseDto.Id);
                    await JS.InvokeVoidAsync("localStorage.setItem", "currentUserName", loginResponseDto.Username);
                    
                }
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
