using API.Models.DTO.BanHang;
using API.Models.Momo;

namespace API.Services.Momo
{
    public interface IMomoService
    {
        Task<MomoCreatePaymentResponseModel> CreatePaymentAsync(
         DatHang.DatHangRequest model);

        MomoExecuteResponseModel PaymentExecuteAsync(IQueryCollection collection);
    }
}
