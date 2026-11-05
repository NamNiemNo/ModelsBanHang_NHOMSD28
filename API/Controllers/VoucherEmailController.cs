using API.Models.DTO.Voucher;
using API.Services.Email;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace API.Controllers
{
    [Route("api/VoucherEmail")]
    [ApiController]
    public class VoucherEmailController : ControllerBase
    {
        private readonly DbContextApp _db;
        private readonly IEmailSender _emailSender;

        public VoucherEmailController(DbContextApp db, IEmailSender emailSender)
        {
            _db = db;
            _emailSender = emailSender;
        }

        [HttpPost("send")]
        public async Task<IActionResult> Send([FromBody] SendVoucherEmailRequest request, CancellationToken cancellationToken)
        {
            if (request.VoucherId == Guid.Empty)
                return BadRequest("Voucher không hợp lệ.");

            var voucher = await _db.Vouchers.AsNoTracking()
                .FirstOrDefaultAsync(x => x.VoucherId == request.VoucherId, cancellationToken);
            if (voucher == null) return NotFound("Không tìm thấy voucher.");
            if (!voucher.TrangThai) return BadRequest("Voucher đang bị tắt.");

            var customersQuery = _db.KhachHangs.AsNoTracking().Where(x => x.TrangThai && x.Email != null && x.Email != "");
            if (!request.SendToAllCustomers)
            {
                var ids = request.CustomerIds?.Where(x => x != Guid.Empty).Distinct().ToList() ?? new();
                if (ids.Count == 0) return BadRequest("Vui lòng chọn ít nhất một khách hàng.");
                customersQuery = customersQuery.Where(x => ids.Contains(x.KhachHangId));
            }

            var customers = await customersQuery.OrderBy(x => x.HoTen).ToListAsync(cancellationToken);
            if (customers.Count == 0) return BadRequest("Không có khách hàng hợp lệ có địa chỉ email.");

            var alreadyAssigned = await _db.KhachHangVouchers
                .Where(x => x.VoucherId == request.VoucherId)
                .Select(x => x.KhachHangId).ToListAsync(cancellationToken);
            var assignedSet = alreadyAssigned.ToHashSet();
            // Không gửi lặp voucher đã cấp; khách gửi thất bại chưa có liên kết nên có thể gửi lại.
            var recipients = customers.Where(x => !assignedSet.Contains(x.KhachHangId)).ToList();
            var sent = new List<Guid>();
            var failed = new List<object>();

            foreach (var customer in recipients)
            {
                try
                {
                    var body = BuildEmail(voucher, customer.HoTen);
                    await _emailSender.SendAsync(customer.Email, $"Bạn nhận được voucher {voucher.TenVoucher} từ NIVO", body, cancellationToken);
                    sent.Add(customer.KhachHangId);
                }
                catch (Exception ex)
                {
                    failed.Add(new { customer.HoTen, customer.Email, Error = ex.Message });
                }
            }

            // Chỉ ghi nhận voucher cho các email gửi thành công; không giới hạn theo Voucher.SoLuong.
            foreach (var customerId in sent)
            {
                _db.KhachHangVouchers.Add(new KhachHangVoucher
                {
                    Id = Guid.NewGuid(), VoucherId = request.VoucherId,
                    KhachHangId = customerId, NgayTao = DateTime.Now, DaSuDung = false
                });
            }
            await _db.SaveChangesAsync(cancellationToken);

            return Ok(new
            {
                Message = $"Gửi thành công {sent.Count}/{recipients.Count} email mới. Bỏ qua {customers.Count - recipients.Count} khách đã được cấp voucher trước đó.",
                SentCount = sent.Count,
                FailedCount = failed.Count,
                SkippedCount = customers.Count - recipients.Count,
                FailedCustomers = failed
            });
        }

        private static string BuildEmail(Voucher voucher, string customerName)
        {
            var discount = voucher.PhanTram > 0
                ? $"Giảm {voucher.PhanTram:0.##}% (tối đa {voucher.GiamToiDa:N0}đ)"
                : $"Giảm trực tiếp {voucher.SoTienGiam:N0}đ";
            return $"<div style='font-family:Arial,sans-serif;max-width:600px;margin:auto;color:#222'>" +
                   $"<h2 style='color:#ea580c'>NIVO - Voucher dành cho bạn</h2>" +
                   $"<p>Xin chào {WebUtility.HtmlEncode(customerName)},</p>" +
                   $"<p>Bạn vừa được tặng voucher <b>{WebUtility.HtmlEncode(voucher.TenVoucher)}</b>.</p>" +
                   $"<div style='padding:18px;background:#fff7ed;border:1px dashed #f97316;border-radius:10px'>" +
                   $"<h2>{WebUtility.HtmlEncode(voucher.TenVoucher)}</h2><p>{discount}</p>" +
                   $"<p>Đơn tối thiểu: {voucher.DieuKienToiThieu:N0}đ</p>" +
                   $"<p>Hiệu lực: {voucher.NgayBatDau:dd/MM/yyyy HH:mm} - {voucher.NgayKetThuc:dd/MM/yyyy HH:mm}</p>" +
                   $"<p>Còn {voucher.SoLuong} lượt sử dụng trên toàn hệ thống tại thời điểm gửi.</p></div>" +
                   "<p>Đăng nhập NIVO để xem và sử dụng voucher. Chúc bạn mua sắm vui vẻ!</p></div>";
        }
    }
}
