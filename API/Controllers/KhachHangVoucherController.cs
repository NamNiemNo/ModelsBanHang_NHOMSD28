using API.IRepository;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KhachHangVoucherController : ControllerBase
    {
        private readonly IKhachHangVoucherRepo _repo;
        private readonly IThongBaoRepository _thongBaoRepo;

        public KhachHangVoucherController(
            IKhachHangVoucherRepo repo,
            IThongBaoRepository thongBaoRepo)
        {
            _repo = repo;
            _thongBaoRepo = thongBaoRepo;
        }

        // ✅ Lấy toàn bộ liên kết
        [HttpGet]
        public async Task<ActionResult<IEnumerable<KhachHangVoucher>>> GetAll()
        {
            var list = await _repo.GetAllAsync();
            return Ok(list);
        }

        // ✅ Lấy theo VoucherId
        [HttpGet("by-voucher/{voucherId}")]
        public async Task<ActionResult<IEnumerable<KhachHangVoucher>>> GetByVoucher(Guid voucherId)
        {
            var list = await _repo.GetByVoucherIdAsync(voucherId);
            return Ok(list);
        }

        // ✅ Lấy theo KhachHangId
        [HttpGet("by-khachhang/{khachHangId}")]
        public async Task<ActionResult<IEnumerable<KhachHangVoucher>>> GetByKhachHang(Guid khachHangId)
        {
            var list = await _repo.GetByKhachHangIdAsync(khachHangId);
            return Ok(list);
        }

        // ✅ Gán 1 voucher cho 1 khách hàng
        [HttpPost("assign-one")]
        public async Task<IActionResult> AssignOne(KhachHangVoucher model)
        {
            var voucher = await _repo.GetVoucherByIdAsync(model.VoucherId);
            if (voucher == null) return NotFound("Voucher không tồn tại.");

            var success = await _repo.AddAsync(model);
            if (!success) return Conflict("Khách hàng đã có voucher này.");

            await _thongBaoRepo.ThemThongBaoAsync($"🎁 Gán voucher {voucher.TenVoucher} cho KH {model.KhachHangId}");
            return Ok("Đã gán voucher.");
        }

        // ✅ Gán 1 voucher cho nhiều khách hàng
        [HttpPost("assign-multiple")]
        public async Task<IActionResult> AssignMultiple(Guid voucherId, List<Guid> khachHangIds)
        {
            var voucher = await _repo.GetVoucherByIdAsync(voucherId);
            if (voucher == null) return NotFound("Voucher không tồn tại.");

            var khachHangChuaCoVoucher = (khachHangIds ?? new List<Guid>()).Distinct().ToList();
            await _repo.AddMultipleAsync(voucherId, khachHangChuaCoVoucher);

            await _thongBaoRepo.ThemThongBaoAsync($"🎁 Cấp voucher {voucher.TenVoucher} cho {khachHangChuaCoVoucher.Count} khách hàng.");
            return Ok(new
            {
                Message = $"Đã xử lý danh sách {khachHangChuaCoVoucher.Count} khách hàng. Số lượt sử dụng không bị thay đổi khi cấp voucher.",
                AssignedCount = khachHangChuaCoVoucher.Count,
                RemainingQuota = voucher.SoLuong
            });
        }

        // ✅ Xoá liên kết
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _repo.DeleteAsync(id);
            if (!result) return NotFound();

            await _thongBaoRepo.ThemThongBaoAsync($"🗑️ Đã xoá liên kết voucher của KHVoucherId: {id}");
            return NoContent();
        }

    }
}
