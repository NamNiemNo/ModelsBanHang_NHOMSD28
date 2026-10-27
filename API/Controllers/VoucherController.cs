using API.IRepository;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VoucherController : ControllerBase
    {
        private readonly IVoucherRepo _voucherRepo;
        private readonly IThongBaoRepository _thongBaoRepository; // ✅ Inject repo thông báo

        public VoucherController(IVoucherRepo voucherRepo, IThongBaoRepository thongBaoRepository)
        {
            _voucherRepo = voucherRepo;
            _thongBaoRepository = thongBaoRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Voucher>>> GetAllVouchers()
        {
            var vouchers = await _voucherRepo.GetAll();
            return Ok(vouchers);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Voucher>> GetVoucherById(Guid id)
        {
            var voucher = await _voucherRepo.GetById(id);
            if (voucher == null) return NotFound();
            return Ok(voucher);
        }

        [HttpPost]
        public async Task<ActionResult<Voucher>> CreateVoucher(Voucher voucher)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            await _voucherRepo.Create(voucher);

            // ✅ Ghi thông báo
            await _thongBaoRepository.ThemThongBaoAsync($"🎁 Thêm voucher mới: {voucher.TenVoucher}");

            return CreatedAtAction(nameof(GetVoucherById), new { id = voucher.VoucherId }, voucher);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVoucher(Guid id, Voucher voucher)
        {
            if (id != voucher.VoucherId) return BadRequest("ID mismatch");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            await _voucherRepo.Update(voucher);

            // ✅ Ghi thông báo
            await _thongBaoRepository.ThemThongBaoAsync($"✏️ Cập nhật voucher: {voucher.TenVoucher}");

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVoucher(Guid id)
        {
            var result = await _voucherRepo.Delete(id);
            if (!result) return NotFound();

            // ✅ Ghi thông báo
            await _thongBaoRepository.ThemThongBaoAsync($"🗑️ Đã xoá voucher có ID: {id}");

            return NoContent();
        }

        [HttpGet("code/{code}")]
        public async Task<IActionResult> GetByCode(string code)
        {
            var voucher = await _voucherRepo.GetByCode(code);

            if (voucher == null)
                return NotFound();

            return Ok(voucher);
        }
    }
}
