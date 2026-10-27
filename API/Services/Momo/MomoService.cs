using API.Models.DTO.BanHang;
using API.Models.Momo;
using Microsoft.Extensions.Options;
using RestSharp;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace API.Services.Momo
{
    public class MomoService : IMomoService
    {
        private readonly IOptions<MomoOptionModel> _options;
        public MomoService(IOptions<MomoOptionModel> options)
        {
            _options = options;
        }
        public async Task<MomoCreatePaymentResponseModel> CreatePaymentAsync(DatHang.DatHangRequest model)
        {
            var orderId = Guid.NewGuid().ToString("N");
            var requestId = Guid.NewGuid().ToString("N");

            var amount = Convert.ToInt64(model.TongTienSauKhiGiam).ToString();

            // ❌ KHÔNG dùng ký tự đặc biệt để tránh lỗi signature
            var orderInfo =
                $"Khach hang {model.TenNguoiNhan} Noi dung {model.GhiChu}";

            var extraData = model.HoaDonId.ToString();

            // ================= RAW DATA (PHẢI KHỚP 100%) =================
            var rawData =
                $"accessKey={_options.Value.AccessKey}" +
                $"&amount={amount}" +
                $"&extraData={extraData}" +
                $"&ipnUrl={_options.Value.NotifyUrl}" +
                $"&orderId={orderId}" +
                $"&orderInfo={orderInfo}" +
                $"&partnerCode={_options.Value.PartnerCode}" +
                $"&redirectUrl={_options.Value.ReturnUrl}" +
                $"&requestId={requestId}" +
                $"&requestType={_options.Value.RequestType}";

            var signature = ComputeHmacSha256(rawData, _options.Value.SecretKey);


            var requestData = new
            {
                partnerCode = _options.Value.PartnerCode,
                partnerName = "Test",
                storeId = "MomoTestStore",

                requestId = requestId,

                amount = amount,

                orderId = orderId,

                orderInfo = orderInfo,

                redirectUrl = _options.Value.ReturnUrl,

                ipnUrl = _options.Value.NotifyUrl,

                lang = "vi",

                requestType = _options.Value.RequestType,

                autoCapture = true,

                extraData = extraData,

                orderGroupId = "",

                signature = signature
            };

            var client = new RestClient(_options.Value.MomoApiUrl);
            var request = new RestRequest() { Method = Method.Post };

            request.AddHeader("Content-Type", "application/json; charset=UTF-8");

            // ✅ FIX QUAN TRỌNG: dùng JsonBody thay vì StringBody
            request.AddJsonBody(requestData);

            var response = await client.ExecuteAsync(request);

            Console.WriteLine("Status: " + response.StatusCode);
            Console.WriteLine("Content:");
            Console.WriteLine(response.Content);
            Console.WriteLine("Error:");
            Console.WriteLine(response.ErrorMessage);

            if (!response.IsSuccessful)
            {
                throw new Exception(
                    $"HTTP {(int)response.StatusCode}\n" +
                    $"Content: {response.Content}\n" +
                    $"Error: {response.ErrorMessage}"
                );
            }

            var result = JsonConvert.DeserializeObject<MomoCreatePaymentResponseModel>(response.Content);

            if (result == null || string.IsNullOrEmpty(result.PayUrl))
                throw new Exception("MoMo không trả PayUrl");

            return result;
        }
        public MomoExecuteResponseModel PaymentExecuteAsync(IQueryCollection collection)
        {
            var amount = collection.First(s => s.Key == "amount").Value;
            var orderInfo = collection.First(s => s.Key == "orderInfo").Value;
            var orderId = collection.First(s => s.Key == "orderId").Value;

            return new MomoExecuteResponseModel()
            {
                Amount = amount,
                OrderId = orderId,
                OrderInfo = orderInfo

            };
        }


        private string ComputeHmacSha256(string message, string secretKey)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secretKey);
            var messageBytes = Encoding.UTF8.GetBytes(message);

            byte[] hashBytes;

            using (var hmac = new HMACSHA256(keyBytes))
            {
                hashBytes = hmac.ComputeHash(messageBytes);
            }

            var hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

            return hashString;
        }
    }
}
