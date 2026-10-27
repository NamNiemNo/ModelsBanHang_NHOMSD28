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
            var currentAssigned = await _repo.GetByVoucherIdAsync(model.VoucherId);
            var voucher = await _repo.GetVoucherByIdAsync(model.VoucherId);

            if (voucher == null) return NotFound("Voucher không tồn tại.");
            if (currentAssigned.Count >= voucher.SoLuong)
                return BadRequest("Số lượng voucher đã được sử dụng hết.");

            var success = await _repo.AddAsync(model);
            if (!success) return Conflict("Khách hàng đã có voucher này.");

            await _thongBaoRepo.ThemThongBaoAsync($"🎁 Gán voucher {voucher.TenVoucher} cho KH {model.KhachHangId}");
            return Ok("Đã gán voucher.");
        }

        // ✅ Gán 1 voucher cho nhiều khách hàng
        [HttpPost("assign-multiple")]
        public async Task<IActionResult> AssignMultiple(Guid voucherId, List<Guid> khachHangIds)
        {
            var currentAssigned = await _repo.GetByVoucherIdAsync(voucherId);
            var voucher = await _repo.GetVoucherByIdAsync(voucherId);

            if (voucher == null) return NotFound("Voucher không tồn tại.");

            int remaining = voucher.SoLuong - currentAssigned.Count;
            if (remaining <= 0) return BadRequest("Số lượng voucher đã hết.");

            // Lọc khách hàng chưa được gán
            var khachHangChuaCoVoucher = khachHangIds
                .Where(khId => !currentAssigned.Any(x => x.KhachHangId == khId))
                .Take(remaining)
                .ToList();

            await _repo.AddMultipleAsync(voucherId, khachHangChuaCoVoucher);

            await _thongBaoRepo.ThemThongBaoAsync($"🎁 Gán voucher {voucher.TenVoucher} cho {khachHangChuaCoVoucher.Count} khách hàng.");
            return Ok(new
            {
                Message = $"Đã gán {khachHangChuaCoVoucher.Count}/{khachHangIds.Count} khách hàng.",
                AssignedCount = khachHangChuaCoVoucher.Count,
                RemainingQuota = remaining - khachHangChuaCoVoucher.Count
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
