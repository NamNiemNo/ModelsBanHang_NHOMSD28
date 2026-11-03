using API.Models.DTO.BanHang;
using API.Models.VNPay;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

public interface IVnPayService
{
    string CreatePaymentUrl(DatHang.DatHangRequest model, HttpContext context, Guid hoaDonId);
    bool ValidateSignature(IQueryCollection collections);
}

public class VnPayService : IVnPayService
{
    private readonly VnPaySettings _vnPaySettings;

    public VnPayService(IOptions<VnPaySettings> vnPaySettings)
    {
        _vnPaySettings = vnPaySettings.Value;
    }

    public string CreatePaymentUrl(DatHang.DatHangRequest model, HttpContext context, Guid hoaDonId)
    {
        var pay = new VnPayLibrary();
        var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
        var tick = DateTime.Now.Ticks.ToString();

        pay.AddRequestData("vnp_Version", _vnPaySettings.Version);
        pay.AddRequestData("vnp_Command", _vnPaySettings.Command);
        pay.AddRequestData("vnp_TmnCode", _vnPaySettings.TmnCode);

        // Số tiền VNPay yêu cầu nhân lên 100 (Ví dụ: 100,000 VNĐ -> 10000000)
        pay.AddRequestData("vnp_Amount", ((long)(model.TongTienSauKhiGiam * 100)).ToString());
        pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
        pay.AddRequestData("vnp_CurrCode", _vnPaySettings.CurrCode);
        pay.AddRequestData("vnp_IpAddr", GetIpAddress(context));
        pay.AddRequestData("vnp_Locale", _vnPaySettings.Locale);
        pay.AddRequestData("vnp_OrderInfo", $"Thanh toan don hang {hoaDonId}");
        pay.AddRequestData("vnp_OrderType", "other");
        pay.AddRequestData("vnp_ReturnUrl", _vnPaySettings.ReturnUrl);
        pay.AddRequestData("vnp_TxnRef", hoaDonId.ToString()); // Dùng HoaDonId làm mã giao dịch

        var paymentUrl = pay.CreateRequestUrl(_vnPaySettings.BaseUrl, _vnPaySettings.HashSecret);
        return paymentUrl;
    }

    public bool ValidateSignature(IQueryCollection collections)
    {
        var pay = new VnPayLibrary();
        foreach (var (key, value) in collections)
        {
            if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
            {
                pay.AddResponseData(key, value!);
            }
        }

        var inputHash = collections["vnp_SecureHash"];
        return pay.ValidateSignature(inputHash!, _vnPaySettings.HashSecret);
    }

    private string GetIpAddress(HttpContext context)
    {
        var ipAddress = string.Empty;
        try
        {
            ipAddress = context.Connection.RemoteIpAddress?.ToString();
            if (string.IsNullOrEmpty(ipAddress) || ipAddress == "::1")
            {
                ipAddress = "127.0.0.1";
            }
        }
        catch
        {
            ipAddress = "127.0.0.1";
        }
        return ipAddress;
    }
}

// Thư viện phụ trợ xử lý dữ liệu cho VNPay
public class VnPayLibrary
{
    private readonly SortedList<string, string> _requestData = new(new VnPayCompare());
    private readonly SortedList<string, string> _responseData = new(new VnPayCompare());

    public void AddRequestData(string key, string value)
    {
        if (!string.IsNullOrEmpty(value)) _requestData.Add(key, value);
    }

    public void AddResponseData(string key, string value)
    {
        if (!string.IsNullOrEmpty(value)) _responseData.Add(key, value);
    }

    public string CreateRequestUrl(string baseUrl, string vnp_HashSecret)
    {
        var data = new StringBuilder();
        foreach (var (key, value) in _requestData)
        {
            if (!string.IsNullOrEmpty(value))
            {
                data.Append(Uri.EscapeDataString(key) + "=" + Uri.EscapeDataString(value) + "&");
            }
        }
        var queryString = data.ToString();
        baseUrl += "?" + queryString;
        var signData = queryString.TrimEnd('&');
        var vnp_SecureHash = HmacSHA512(vnp_HashSecret, signData);
        baseUrl += "vnp_SecureHash=" + vnp_SecureHash;
        return baseUrl;
    }

    public bool ValidateSignature(string inputHash, string secretKey)
    {
        var rspRaw = new StringBuilder();
        foreach (var (key, value) in _responseData)
        {
            if (!string.IsNullOrEmpty(value) && key != "vnp_SecureHash")
            {
                rspRaw.Append(Uri.EscapeDataString(key) + "=" + Uri.EscapeDataString(value) + "&");
            }
        }
        var data = rspRaw.ToString().TrimEnd('&');
        var myChecksum = HmacSHA512(secretKey, data);
        return myChecksum.Equals(inputHash, StringComparison.InvariantCultureIgnoreCase);
    }

    private string HmacSHA512(string key, string inputData)
    {
        var hash = new StringBuilder();
        var keyBytes = Encoding.UTF8.GetBytes(key);
        var inputBytes = Encoding.UTF8.GetBytes(inputData);
        using (var hmac = new HMACSHA512(keyBytes))
        {
            var hashValue = hmac.ComputeHash(inputBytes);
            foreach (var b in hashValue)
            {
                hash.Append(b.ToString("x2"));
            }
        }
        return hash.ToString();
    }
}

public class VnPayCompare : IComparer<string>
{
    public int Compare(string? x, string? y)
    {
        if (x == y) return 0;
        if (x == null) return -1;
        if (y == null) return 1;
        return string.Compare(x, y, StringComparison.Ordinal);
    }
}