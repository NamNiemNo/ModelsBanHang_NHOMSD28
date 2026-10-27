using API.IRepository;
using API.IRepository.Repository;
using API.Models.DTO.BanHang;
using API.Services.Shipping;
using Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HoaDonController : ControllerBase
    {
        private static readonly Guid CodPaymentId =
            Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

        private readonly IHoaDonRepo _hoaDonRepo;
        private readonly IThongBaoRepository _thongBaoRepo;
        private readonly IChiTietHoaDonRepository _chiTietHoaDonRepo;
        private readonly IGioHangChiTietRepository _gioHangChiTietRepo;
        private readonly IVoucherRepo _voucherRepo;
        private readonly IShippingFeeService _shippingFeeService;

        public HoaDonController(
            IHoaDonRepo hoaDonRepo,
            IThongBaoRepository thongBaoRepo,
            IChiTietHoaDonRepository chiTietHoaDonRepo,
            IGioHangChiTietRepository gioHangChiTietRepo,
            IVoucherRepo voucherRepo,
            IShippingFeeService shippingFeeService)
        {
            _hoaDonRepo = hoaDonRepo;
            _thongBaoRepo = thongBaoRepo;
            _chiTietHoaDonRepo = chiTietHoaDonRepo;
            _gioHangChiTietRepo = gioHangChiTietRepo;
            _voucherRepo = voucherRepo;
            _shippingFeeService = shippingFeeService;
        }

        // =====================================================
        // LẤY TOÀN BỘ HÓA ĐƠN
        // GET: api/HoaDon
        // =====================================================

        [HttpGet]
        public async Task<ActionResult<IEnumerable<HoaDon>>>
            GetAllHoaDons()
        {
            var hoaDons = await _hoaDonRepo.GetAll();

            return Ok(hoaDons);
        }

        // =====================================================
        // LẤY HÓA ĐƠN THEO ID
        // GET: api/HoaDon/{id}
        // =====================================================

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<HoaDon>>
            GetHoaDonById(Guid id)
        {
            var hoaDon = await _hoaDonRepo.GetById(id);

            if (hoaDon == null)
            {
                return NotFound(
                    "Không tìm thấy hóa đơn.");
            }

            return Ok(hoaDon);
        }

        // =====================================================
        // TẠO HÓA ĐƠN TỪ TRANG BÁN HÀNG TẠI QUẦY
        // POST: api/HoaDon
        // =====================================================

        [HttpPost]
        public async Task<ActionResult<HoaDon>>
            CreateHoaDon([FromBody] HoaDon hoaDon)
        {
            if (hoaDon == null)
            {
                return BadRequest(
                    "Dữ liệu hóa đơn không hợp lệ.");
            }

            await _hoaDonRepo.Create(hoaDon);

            await _thongBaoRepo.ThemThongBaoAsync(
                $"🧾 Đã tạo hóa đơn mới với mã: {hoaDon.HoaDonId}");

            return CreatedAtAction(
                nameof(GetHoaDonById),
                new
                {
                    id = hoaDon.HoaDonId
                },
                hoaDon);
        }

        // =====================================================
        // CẬP NHẬT HÓA ĐƠN
        // PUT: api/HoaDon/{id}
        // =====================================================

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateHoaDon(
            Guid id,
            [FromBody] HoaDon hoaDon)
        {
            if (hoaDon == null)
            {
                return BadRequest(
                    "Dữ liệu hóa đơn không hợp lệ.");
            }

            if (id != hoaDon.HoaDonId)
            {
                return BadRequest(
                    "ID hóa đơn không khớp.");
            }

            await _hoaDonRepo.Update(hoaDon);

            await _thongBaoRepo.ThemThongBaoAsync(
                $"✏️ Đã cập nhật hóa đơn: {hoaDon.HoaDonId}");

            return NoContent();
        }

        // =====================================================
        // XÓA HÓA ĐƠN
        // DELETE: api/HoaDon/{id}
        // =====================================================

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteHoaDon(Guid id)
        {
            var hoaDon = await _hoaDonRepo.GetById(id);

            if (hoaDon == null)
            {
                return NotFound(
                    "Không tìm thấy hóa đơn.");
            }

            var result = await _hoaDonRepo.Delete(id);

            if (!result)
            {
                return BadRequest(
                    "Không thể xóa hóa đơn.");
            }

            await _thongBaoRepo.ThemThongBaoAsync(
                $"🗑️ Đã xóa hóa đơn: {hoaDon.HoaDonId}");

            return NoContent();
        }

        // =====================================================
        // API TÍNH PHÍ VẬN CHUYỂN
        // POST: api/HoaDon/tinh-phi-ship
        // =====================================================

        [HttpPost("tinh-phi-ship")]
        public ActionResult<DatHang.TinhPhiShipResponse>
            TinhPhiShip(
                [FromBody] DatHang.TinhPhiShipRequest request)
        {
            if (request == null)
            {
                return BadRequest(
                    "Dữ liệu tính phí ship không hợp lệ.");
            }

            if (request.TamTinh < 0)
            {
                return BadRequest(
                    "Tạm tính không được nhỏ hơn 0.");
            }

            if (string.IsNullOrWhiteSpace(
                request.TinhThanhPho))
            {
                return BadRequest(
                    "Vui lòng chọn tỉnh/thành phố.");
            }

            var result = _shippingFeeService.Calculate(
                request.TinhThanhPho,
                request.TamTinh);

            return Ok(result);
        }

        // =====================================================
        // KHÁCH HÀNG ĐẶT HÀNG COD
        // POST: api/HoaDon/dat-hang
        // =====================================================

        [HttpPost("dat-hang")]
        public async Task<IActionResult> DatHang(
            [FromBody] DatHang.DatHangRequest request)
        {
            if (request == null)
            {
                return BadRequest(
                    "Dữ liệu đặt hàng không hợp lệ.");
            }

            if (request.KhachHangId == Guid.Empty)
            {
                return BadRequest(
                    "Không xác định được khách hàng.");
            }

            if (request.Items == null ||
                request.Items.Count == 0)
            {
                return BadRequest(
                    "Không có sản phẩm để đặt hàng.");
            }

            if (request.Items.Any(item =>
                    item.GiayChiTietId == Guid.Empty ||
                    item.SoLuong <= 0 ||
                    item.Gia < 0))
            {
                return BadRequest(
                    "Sản phẩm, số lượng hoặc giá không hợp lệ.");
            }

            // Tạm tính được API tính lại từ danh sách sản phẩm.
            // API không sử dụng tổng tiền do trình duyệt gửi lên.
            decimal tamTinh = request.Items.Sum(
                item => item.Gia * item.SoLuong);

            string tinhThanhPho =
                ResolveProvince(request);

            if (string.IsNullOrWhiteSpace(tinhThanhPho))
            {
                return BadRequest(
                    "Không xác định được tỉnh/thành phố giao hàng.");
            }

            // API tự tính phí ship.
            var shippingQuote =
                _shippingFeeService.Calculate(
                    tinhThanhPho,
                    tamTinh);

            Voucher? voucher = null;

            if (request.VoucherId.HasValue)
            {
                voucher = await _voucherRepo.GetById(
                    request.VoucherId.Value);

                string voucherError =
                    ValidateVoucher(voucher, tamTinh);

                if (!string.IsNullOrWhiteSpace(voucherError))
                {
                    return BadRequest(voucherError);
                }
            }

            decimal giamGia =
                CalculateDiscount(tamTinh, voucher);

            decimal tongThanhToan = Math.Max(
                0m,
                tamTinh +
                shippingQuote.PhiShip -
                giamGia);

            Guid hinhThucThanhToanId =
                request.HinhThucThanhToanId == Guid.Empty
                    ? CodPaymentId
                    : request.HinhThucThanhToanId;

            var hoaDon = new HoaDon
            {
                HoaDonId = Guid.NewGuid(),

                KhachHangId =
                    request.KhachHangId,

                NhanVienId = null,

                NgayTao = DateTime.Now,

                TrangThai = "Chờ xác nhận",

                HinhThucThanhToanId =
                    hinhThucThanhToanId,

                TrangThaiThanhToan =
                    hinhThucThanhToanId == CodPaymentId
                        ? "Chờ thanh toán"
                        : "Đã thanh toán",

                TenNguoiNhan =
                    request.TenNguoiNhan?.Trim() ?? "",

                SoDienThoai =
                    request.SoDienThoai?.Trim() ?? "",

                DiaChi =
                    request.DiaChi?.Trim() ?? "",

                VoucherId =
                    request.VoucherId,

                // Hai giá trị quan trọng được API tự tính.
                PhiShip =
                    (float)shippingQuote.PhiShip,

                TongTienSauKhiGiam =
                    (float)tongThanhToan,

                GhiChu =
                    request.GhiChu?.Trim()
            };

            var chiTiets = request.Items
                .Select(item => new HoaDonChiTiet
                {
                    HoaDonChiTietId =
                        Guid.NewGuid(),

                    GiayChiTietId =
                        item.GiayChiTietId,

                    SoLuongSanPham =
                        item.SoLuong,

                    DonGia =
                        item.Gia,

                    Gia =
                        item.Gia * item.SoLuong,

                    TrangThai = false
                })
                .ToList();

            var gioHangIds = request.Items
                .Where(item =>
                    item.GioHangChiTietId != Guid.Empty)
                .Select(item =>
                    item.GioHangChiTietId)
                .Distinct()
                .ToList();

            var result = await _hoaDonRepo.DatHangAsync(
                hoaDon,
                chiTiets,
                gioHangIds);

            if (!result)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Đặt hàng thất bại.");
            }

            await _thongBaoRepo.ThemThongBaoAsync(
                $"🧾 Đơn hàng mới chờ xác nhận: {hoaDon.HoaDonId}");

            return Ok(new
            {
                hoaDon.HoaDonId,
                hoaDon.TrangThai,
                hoaDon.TrangThaiThanhToan,
                hoaDon.PhiShip,
                hoaDon.TongTienSauKhiGiam
            });
        }

        // =====================================================
        // LẤY ĐƠN HÀNG THEO KHÁCH HÀNG
        // GET: api/HoaDon/khach-hang/{khachHangId}
        // =====================================================

        [HttpGet("khach-hang/{khachHangId:guid}")]
        public async Task<IActionResult>
            GetHoaDonByKhachHang(Guid khachHangId)
        {
            try
            {
                var hoaDons =
                    await _hoaDonRepo.GetByKhachHangId(
                        khachHangId);

                if (hoaDons == null ||
                    !hoaDons.Any())
                {
                    return Ok(new List<object>());
                }

                var result = hoaDons.Select(hoaDon => new
                {
                    HoaDonId =
                        hoaDon.HoaDonId,

                    TrangThai =
                        hoaDon.TrangThai,

                    TrangThaiThanhToan =
                        hoaDon.TrangThaiThanhToan,

                    NgayTao =
                        hoaDon.NgayTao,

                    HinhThucThanhToan =
                        hoaDon.hinhThucThanhToan != null
                            ? hoaDon.hinhThucThanhToan.TenHinhThuc
                            : "",

                    TenNguoiNhan =
                        hoaDon.TenNguoiNhan,

                    SoDienThoai =
                        hoaDon.SoDienThoai,

                    DiaChi =
                        hoaDon.DiaChi,

                    // Trả phí ship về trang đơn mua.
                    PhiShip =
                        hoaDon.PhiShip,

                    // Tổng tiền này đã bao gồm phí ship.
                    TongTien =
                        hoaDon.TongTienSauKhiGiam,

                    ChiTiet = hoaDon.HoaDonChiTiets
                        .Select(chiTiet => new
                        {
                            GiayChiTietId =
                                chiTiet.GiayChiTietId,

                            SoLuongSanPham =
                                chiTiet.SoLuongSanPham,

                            DonGia =
                                chiTiet.DonGia ?? 0m,

                            Gia =
                                chiTiet.Gia,

                            TenGiay =
                                chiTiet.GiayChiTiet
                                    .Giay.TenGiay,

                            Size =
                                chiTiet.GiayChiTiet
                                    .KichCo != null
                                        ? chiTiet.GiayChiTiet
                                            .KichCo.TenKichCo
                                        : "",

                            MauSac =
                                chiTiet.GiayChiTiet
                                    .MauSac != null
                                        ? chiTiet.GiayChiTiet
                                            .MauSac.TenMau
                                        : "",

                            Anh =
                                chiTiet.GiayChiTiet
                                    .Anhs.FirstOrDefault()
                                    ?.DuongDan
                        })
                        .ToList()
                });

                return Ok(result);
            }
            catch (Exception exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    exception.Message);
            }
        }

        // =====================================================
        // CẬP NHẬT THÔNG TIN GIAO HÀNG
        // PUT: api/HoaDon/cap-nhat-giao-hang
        // =====================================================

        [HttpPut("cap-nhat-giao-hang")]
        public async Task<IActionResult>
            CapNhatThongTinGiaoHang(
                [FromBody] UpdateThongTinGiaoHangDTO dto)
        {
            if (dto == null)
            {
                return BadRequest(
                    "Dữ liệu giao hàng không hợp lệ.");
            }

            var hoaDon =
                await _hoaDonRepo.GetById(dto.HoaDonId);

            if (hoaDon == null)
            {
                return NotFound(
                    "Không tìm thấy hóa đơn.");
            }

            if (!string.IsNullOrWhiteSpace(
                dto.TenNguoiNhan))
            {
                hoaDon.TenNguoiNhan =
                    dto.TenNguoiNhan.Trim();
            }

            if (!string.IsNullOrWhiteSpace(
                dto.SoDienThoai))
            {
                hoaDon.SoDienThoai =
                    dto.SoDienThoai.Trim();
            }

            if (!string.IsNullOrWhiteSpace(dto.DiaChi))
            {
                hoaDon.DiaChi =
                    dto.DiaChi.Trim();
            }

            await _hoaDonRepo.Update(hoaDon);

            await _thongBaoRepo.ThemThongBaoAsync(
                $"✏️ Khách sửa thông tin giao hàng: {hoaDon.HoaDonId}");

            return Ok(new
            {
                message =
                    "Cập nhật thông tin giao hàng thành công."
            });
        }

        // =====================================================
        // HỦY ĐƠN
        // PUT: api/HoaDon/huy-don/{hoaDonId}
        // =====================================================

        [HttpPut("huy-don/{hoaDonId:guid}")]
        public async Task<IActionResult> HuyDon(
            Guid hoaDonId,
            [FromQuery] string? lyDo)
        {
            try
            {
                var result =
                    await _hoaDonRepo.HuyDonHangAsync(
                        hoaDonId,
                        lyDo);

                if (!result)
                {
                    return BadRequest(
                        "Không thể hủy đơn.");
                }

                await _thongBaoRepo.ThemThongBaoAsync(
                    $"❌ Đơn hàng đã bị hủy: {hoaDonId} - " +
                    $"Lý do: {lyDo ?? "Admin hủy"}");

                return Ok(new
                {
                    message =
                        "Hủy đơn hàng thành công."
                });
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        // =====================================================
        // LẤY CHI TIẾT HÓA ĐƠN
        // GET: api/HoaDon/chi-tiet/{hoaDonId}
        // =====================================================

        [HttpGet("chi-tiet/{hoaDonId:guid}")]
        public async Task<IActionResult>
            GetHoaDonChiTiet(Guid hoaDonId)
        {
            var hoaDon =
                await _hoaDonRepo.GetById(hoaDonId);

            if (hoaDon == null)
            {
                return NotFound(
                    "Không tìm thấy hóa đơn.");
            }

            var result = new HoaDonKhachHangDTO
            {
                HoaDonId =
                    hoaDon.HoaDonId,

                TrangThai =
                    hoaDon.TrangThai,

                TrangThaiThanhToan =
                    hoaDon.TrangThaiThanhToan,

                NgayTao =
                    hoaDon.NgayTao,

                TenNguoiNhan =
                    hoaDon.TenNguoiNhan,

                SoDienThoai =
                    hoaDon.SoDienThoai,

                DiaChi =
                    hoaDon.DiaChi,

                PhiShip =
                    (decimal)hoaDon.PhiShip,

                GiamGiaVoucher =
                    hoaDon.voucher != null
                        ? CalculateDiscountFromInvoice(
                            hoaDon)
                        : 0m,

                TongTien =
                    Convert.ToDecimal(
                        hoaDon.TongTienSauKhiGiam),

                HinhThucThanhToan =
                    hoaDon.hinhThucThanhToan != null
                        ? hoaDon.hinhThucThanhToan
                            .TenHinhThuc
                        : "",

                ChiTiet = hoaDon.HoaDonChiTiets
                    .Select(chiTiet =>
                        new HoaDonChiTietDTO
                        {
                            GiayChiTietId =
                                chiTiet.GiayChiTietId,

                            SoLuongSanPham =
                                chiTiet.SoLuongSanPham,

                            Gia =
                                chiTiet.Gia,

                            DonGia =
                                chiTiet.DonGia ?? 0m,

                            TenGiay =
                                chiTiet.GiayChiTiet
                                    .Giay.TenGiay,

                            Anh =
                                chiTiet.GiayChiTiet
                                    .Anhs.FirstOrDefault()
                                    ?.DuongDan,

                            Size =
                                chiTiet.GiayChiTiet
                                    .KichCo?.TenKichCo,

                            MauSac =
                                chiTiet.GiayChiTiet
                                    .MauSac?.TenMau
                        })
                    .ToList()
            };

            return Ok(result);
        }

        // =====================================================
        // GIAO HÀNG THÀNH CÔNG
        // PUT: api/HoaDon/giao-thanh-cong/{id}
        // =====================================================

        [HttpPut("giao-thanh-cong/{id:guid}")]
        public async Task<IActionResult>
            GiaoThanhCong(Guid id)
        {
            var result =
                await _hoaDonRepo.GiaoThanhCongAsync(id);

            if (!result)
            {
                return BadRequest(
                    "Không thể cập nhật đơn.");
            }

            await _thongBaoRepo.ThemThongBaoAsync(
                $"✅ Đơn hàng hoàn thành: {id}");

            return Ok(new
            {
                message =
                    "Cập nhật giao hàng thành công."
            });
        }

        // =====================================================
        // XÁC NHẬN ĐƠN
        // PUT: api/HoaDon/xac-nhan/{id}
        // =====================================================

        [HttpPut("xac-nhan/{id:guid}")]
        public async Task<IActionResult>
            XacNhanDon(Guid id)
        {
            var result =
                await _hoaDonRepo.XacNhanDonAsync(id);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            await _thongBaoRepo.ThemThongBaoAsync(
                $"📦 Đã xác nhận đơn: {id}");

            return Ok(result.Message);
        }

        // =====================================================
        // BẮT ĐẦU GIAO HÀNG
        // PUT: api/HoaDon/bat-dau-giao/{id}
        // =====================================================

        [HttpPut("bat-dau-giao/{id:guid}")]
        public async Task<IActionResult>
            BatDauGiao(Guid id)
        {
            var result =
                await _hoaDonRepo.BatDauGiaoHangAsync(id);

            if (!result)
            {
                return BadRequest(
                    "Không thể chuyển trạng thái.");
            }

            await _thongBaoRepo.ThemThongBaoAsync(
                $"🚚 Đơn hàng đang giao: {id}");

            return Ok(new
            {
                message =
                    "Đã chuyển đơn sang trạng thái đang giao."
            });
        }

        // =====================================================
        // DUYỆT YÊU CẦU HỦY ĐƠN
        // PUT: api/HoaDon/duyet-huy/{hoaDonId}
        // =====================================================

        [HttpPut("duyet-huy/{hoaDonId:guid}")]
        public async Task<IActionResult> DuyetHuy(
            Guid hoaDonId,
            [FromQuery] bool chapNhan)
        {
            try
            {
                var result =
                    await _hoaDonRepo.DuyetHuyDonAsync(
                        hoaDonId,
                        chapNhan);

                if (!result)
                {
                    return NotFound(
                        "Không tìm thấy đơn hàng.");
                }

                return Ok(new
                {
                    message = chapNhan
                        ? "Đã chấp nhận hủy đơn."
                        : "Đã từ chối hủy đơn."
                });
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        // =====================================================
        // HÀM HỖ TRỢ
        // =====================================================

        private static string ResolveProvince(
            DatHang.DatHangRequest request)
        {
            if (!string.IsNullOrWhiteSpace(
                request.TinhThanhPho))
            {
                return request.TinhThanhPho.Trim();
            }

            if (string.IsNullOrWhiteSpace(request.DiaChi))
            {
                return "";
            }

            return request.DiaChi
                .Split(
                    ',',
                    StringSplitOptions.RemoveEmptyEntries |
                    StringSplitOptions.TrimEntries)
                .LastOrDefault()
                ?? "";
        }

        private static string ValidateVoucher(
            Voucher? voucher,
            decimal tamTinh)
        {
            if (voucher == null)
            {
                return "Voucher không tồn tại.";
            }

            if (!voucher.TrangThai)
            {
                return "Voucher đang bị khóa.";
            }

            if (DateTime.Now < voucher.NgayBatDau)
            {
                return "Voucher chưa đến thời gian sử dụng.";
            }

            if (DateTime.Now > voucher.NgayKetThuc)
            {
                return "Voucher đã hết hạn.";
            }

            if (voucher.SoLuong <= 0)
            {
                return "Voucher đã hết lượt sử dụng.";
            }

            if (tamTinh <
                (decimal)voucher.DieuKienToiThieu)
            {
                return
                    $"Đơn hàng phải đạt tối thiểu " +
                    $"{voucher.DieuKienToiThieu:N0} VNĐ.";
            }

            if (voucher.PhanTram < 0 ||
                voucher.PhanTram > 100)
            {
                return
                    "Phần trăm giảm giá không hợp lệ.";
            }

            if (voucher.PhanTram <= 0 &&
                voucher.SoTienGiam <= 0)
            {
                return
                    "Voucher không có giá trị giảm.";
            }

            return "";
        }

        private static decimal CalculateDiscount(
            decimal tamTinh,
            Voucher? voucher)
        {
            if (voucher == null)
            {
                return 0m;
            }

            decimal discount;

            if (voucher.PhanTram > 0)
            {
                decimal percent = Math.Min(
                    (decimal)voucher.PhanTram,
                    100m);

                discount =
                    tamTinh * percent / 100m;
            }
            else
            {
                discount =
                    (decimal)voucher.SoTienGiam;
            }

            if (voucher.GiamToiDa > 0)
            {
                discount = Math.Min(
                    discount,
                    (decimal)voucher.GiamToiDa);
            }

            return Math.Min(
                tamTinh,
                Math.Max(0m, discount));
        }

        private static decimal
            CalculateDiscountFromInvoice(HoaDon hoaDon)
        {
            if (hoaDon.voucher == null)
            {
                return 0m;
            }

            decimal subtotal =
                hoaDon.HoaDonChiTiets.Sum(
                    chiTiet => chiTiet.Gia);

            return CalculateDiscount(
                subtotal,
                hoaDon.voucher);
        }
    }
}