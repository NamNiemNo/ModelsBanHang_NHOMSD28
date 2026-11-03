using API.IRepository;
using API.Models.DTO.BanHang;
using API.Services.Shipping;
using Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IVnPayService _vnPayService;
        private readonly IHoaDonRepo _hoaDonRepo;
        private readonly IVoucherRepo _voucherRepo;
        private readonly IShippingFeeService _shippingFeeService;

        public PaymentController(
            IVnPayService vnPayService,
            IHoaDonRepo hoaDonRepo,
            IVoucherRepo voucherRepo,
            IShippingFeeService shippingFeeService)
        {
            _vnPayService = vnPayService;
            _hoaDonRepo = hoaDonRepo;
            _voucherRepo = voucherRepo;
            _shippingFeeService = shippingFeeService;
        }

        // =====================================================
        // TẠO HÓA ĐƠN VÀ URL THANH TOÁN VNPAY
        // POST: api/Payment/CreateVnPayPayment
        // =====================================================
        [HttpPost("CreateVnPayPayment")]
        public async Task<IActionResult> CreateVnPayPayment([FromBody] DatHang.DatHangRequest request)
        {
            var pricingResult = await CalculateOrderPricingAsync(request);
            if (!pricingResult.Success)
            {
                return BadRequest(pricingResult.Error);
            }

            request.PhiShip = (float)pricingResult.PhiShip;
            request.TongTienSauKhiGiam = (float)pricingResult.TongThanhToan;

            // 1. Tạo hóa đơn trước vào DB để lấy HoaDonId làm mã giao dịch (vnp_TxnRef)
            var hoaDon = new HoaDon
            {
                HoaDonId = Guid.NewGuid(),
                KhachHangId = request.KhachHangId,
                NgayTao = DateTime.Now,
                TrangThai = "Chờ xác nhận",
                TrangThaiThanhToan = "Chờ thanh toán",
                HinhThucThanhToanId = request.HinhThucThanhToanId,
                TenNguoiNhan = request.TenNguoiNhan?.Trim() ?? "",
                SoDienThoai = request.SoDienThoai?.Trim() ?? "",
                DiaChi = request.DiaChi?.Trim() ?? "",
                VoucherId = request.VoucherId,
                PhiShip = (float)pricingResult.PhiShip,
                TongTienSauKhiGiam = (float)pricingResult.TongThanhToan,
                GhiChu = request.GhiChu?.Trim()
            };

            var chiTiets = request.Items.Select(item => new HoaDonChiTiet
            {
                HoaDonChiTietId = Guid.NewGuid(),
                GiayChiTietId = item.GiayChiTietId,
                SoLuongSanPham = item.SoLuong,
                DonGia = item.Gia,
                Gia = item.Gia * item.SoLuong,
                TrangThai = false
            }).ToList();

            var gioHangIds = request.Items.Where(i => i.GioHangChiTietId != Guid.Empty).Select(i => i.GioHangChiTietId).Distinct().ToList();

            var createOrderResult = await _hoaDonRepo.DatHangAsync(hoaDon, chiTiets, gioHangIds);
            if (!createOrderResult)
            {
                return BadRequest("Không tạo được hóa đơn.");
            }

            request.HoaDonId = hoaDon.HoaDonId;

            // 2. Tạo URL thanh toán VNPay
            var paymentUrl = _vnPayService.CreatePaymentUrl(request, HttpContext, hoaDon.HoaDonId);

            return Ok(new { PayUrl = paymentUrl });
        }

        // =====================================================
        // VNPAY GỬI KẾT QUẢ VỀ (RETURN URL)
        // GET: api/Payment/vnpay-return
        // =====================================================
        [HttpGet("vnpay-return")]
        public async Task<IActionResult> VnPayReturn()
        {
            if (_vnPayService.ValidateSignature(Request.Query))
            {
                var vnp_ResponseCode = Request.Query["vnp_ResponseCode"];
                var vnp_TxnRef = Request.Query["vnp_TxnRef"]; // Đây chính là HoaDonId

                if (vnp_ResponseCode == "00" && Guid.TryParse(vnp_TxnRef, out Guid hoaDonId))
                {
                    var hoaDon = await _hoaDonRepo.GetById(hoaDonId);
                    if (hoaDon != null)
                    {
                        hoaDon.TrangThaiThanhToan = "Đã thanh toán";
                        await _hoaDonRepo.Update(hoaDon);
                    }
                    // Redirect về trang thành công của Blazor (Đổi port nếu Blazor chạy port khác)
                    return Redirect("https://localhost:7001/order-success");
                }
            }

            // Redirect về trang thất bại
            return Redirect("https://localhost:7001/order-failed");
        }

        // =====================================================
        // XÁC NHẬN THANH TOÁN
        // POST: api/Payment/ConfirmPayment/{hoaDonId}
        // =====================================================
        [HttpPost("ConfirmPayment/{hoaDonId:guid}")]
        public async Task<IActionResult> ConfirmPayment(Guid hoaDonId)
        {
            var hoaDon = await _hoaDonRepo.GetById(hoaDonId);
            if (hoaDon == null)
            {
                return NotFound("Không tìm thấy hóa đơn.");
            }

            hoaDon.TrangThaiThanhToan = "Đã thanh toán";
            await _hoaDonRepo.Update(hoaDon);

            return Ok(new { message = "Xác nhận thanh toán thành công." });
        }

        // =====================================================
        // HỦY THANH TOÁN
        // POST: api/Payment/CancelPayment/{hoaDonId}
        // =====================================================
        [HttpPost("CancelPayment/{hoaDonId:guid}")]
        public async Task<IActionResult> CancelPayment(Guid hoaDonId)
        {
            var hoaDon = await _hoaDonRepo.GetById(hoaDonId);
            if (hoaDon == null)
            {
                return NotFound("Không tìm thấy hóa đơn.");
            }

            bool deleteResult = await _hoaDonRepo.Delete(hoaDonId);
            if (!deleteResult)
            {
                return BadRequest("Không thể hủy thanh toán.");
            }

            return Ok(new { message = "Đã hủy thanh toán." });
        }

        // =====================================================
        // API TỰ TÍNH PHÍ SHIP, VOUCHER VÀ TỔNG TIỀN
        // =====================================================
        private async Task<OrderPricingResult> CalculateOrderPricingAsync(DatHang.DatHangRequest request)
        {
            if (request == null)
            {
                return OrderPricingResult.Fail("Dữ liệu thanh toán không hợp lệ.");
            }

            if (request.KhachHangId == Guid.Empty)
            {
                return OrderPricingResult.Fail("Không xác định được khách hàng.");
            }

            if (request.Items == null || request.Items.Count == 0)
            {
                return OrderPricingResult.Fail("Không có sản phẩm để thanh toán.");
            }

            if (request.Items.Any(item => item.GiayChiTietId == Guid.Empty || item.SoLuong <= 0 || item.Gia < 0))
            {
                return OrderPricingResult.Fail("Sản phẩm, số lượng hoặc giá không hợp lệ.");
            }

            decimal tamTinh = request.Items.Sum(item => item.Gia * item.SoLuong);

            string tinhThanhPho = ResolveProvince(request);

            if (string.IsNullOrWhiteSpace(tinhThanhPho))
            {
                return OrderPricingResult.Fail("Không xác định được tỉnh/thành phố giao hàng.");
            }

            var shippingQuote = _shippingFeeService.Calculate(tinhThanhPho, tamTinh);

            Voucher? voucher = null;

            if (request.VoucherId.HasValue)
            {
                voucher = await _voucherRepo.GetById(request.VoucherId.Value);

                string voucherError = ValidateVoucher(voucher, tamTinh);

                if (!string.IsNullOrWhiteSpace(voucherError))
                {
                    return OrderPricingResult.Fail(voucherError);
                }
            }

            decimal discount = CalculateDiscount(tamTinh, voucher);

            decimal tongThanhToan = Math.Max(0m, tamTinh + shippingQuote.PhiShip - discount);

            return OrderPricingResult.Ok(shippingQuote.PhiShip, tongThanhToan);
        }

        private static string ResolveProvince(DatHang.DatHangRequest request)
        {
            if (!string.IsNullOrWhiteSpace(request.TinhThanhPho))
            {
                return request.TinhThanhPho.Trim();
            }

            if (string.IsNullOrWhiteSpace(request.DiaChi))
            {
                return "";
            }

            return request.DiaChi
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .LastOrDefault() ?? "";
        }

        private static string ValidateVoucher(Voucher? voucher, decimal tamTinh)
        {
            if (voucher == null) return "Voucher không tồn tại.";
            if (!voucher.TrangThai) return "Voucher đang bị khóa.";
            if (DateTime.Now < voucher.NgayBatDau) return "Voucher chưa đến thời gian sử dụng.";
            if (DateTime.Now > voucher.NgayKetThuc) return "Voucher đã hết hạn.";
            if (voucher.SoLuong <= 0) return "Voucher đã hết lượt sử dụng.";
            if (tamTinh < (decimal)voucher.DieuKienToiThieu) return $"Đơn hàng phải đạt tối thiểu {voucher.DieuKienToiThieu:N0} VNĐ.";
            if (voucher.PhanTram < 0 || voucher.PhanTram > 100) return "Phần trăm giảm giá không hợp lệ.";
            if (voucher.PhanTram <= 0 && voucher.SoTienGiam <= 0) return "Voucher không có giá trị giảm.";

            return "";
        }

        private static decimal CalculateDiscount(decimal tamTinh, Voucher? voucher)
        {
            if (voucher == null) return 0m;

            decimal discount;
            if (voucher.PhanTram > 0)
            {
                decimal percent = Math.Min((decimal)voucher.PhanTram, 100m);
                discount = tamTinh * percent / 100m;
            }
            else
            {
                discount = (decimal)voucher.SoTienGiam;
            }

            if (voucher.GiamToiDa > 0)
            {
                discount = Math.Min(discount, (decimal)voucher.GiamToiDa);
            }

            return Math.Min(tamTinh, Math.Max(0m, discount));
        }

        private sealed class OrderPricingResult
        {
            public bool Success { get; private set; }
            public string Error { get; private set; } = "";
            public decimal PhiShip { get; private set; }
            public decimal TongThanhToan { get; private set; }

            public static OrderPricingResult Ok(decimal phiShip, decimal tongThanhToan)
            {
                return new OrderPricingResult { Success = true, PhiShip = phiShip, TongThanhToan = tongThanhToan };
            }

            public static OrderPricingResult Fail(string error)
            {
                return new OrderPricingResult { Success = false, Error = error, PhiShip = 0m, TongThanhToan = 0m };
            }
        }
    }
}