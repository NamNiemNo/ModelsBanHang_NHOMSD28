using API.IRepository;
using API.Models.DTO;
using Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MauSacController : ControllerBase
    {
        private readonly IMauSacRepository _mauSacRepo;
        private readonly IThongBaoRepository _thongBaoRepo; // ✅ Thêm repo thông báo

        public MauSacController(IMauSacRepository mauSacRepo, IThongBaoRepository thongBaoRepo)
        {
            _mauSacRepo = mauSacRepo;
            _thongBaoRepo = thongBaoRepo;
        }

        private MauSacDTO MapToDTO(MauSac ms) => new MauSacDTO
        {
            MauSacId = ms.MauSacId,
            TenMau = ms.TenMau,
            MoTa = ms.MoTa,
            TrangThai = ms.TrangThai
        };

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mauSacRepo.GetAllAsync();
            return Ok(result.Select(MapToDTO));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var ms = await _mauSacRepo.GetByIdAsync(id);
            if (ms == null) return NotFound();
            return Ok(MapToDTO(ms));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MauSacDTO dto)
        {
            var entity = new MauSac
            {
                TenMau = dto.TenMau,
                MoTa = dto.MoTa,
                TrangThai = dto.TrangThai
            };

            var created = await _mauSacRepo.AddAsync(entity);

            // ✅ Ghi thông báo
            await _thongBaoRepo.ThemThongBaoAsync($"🎨 Đã thêm màu sắc: {created.TenMau}");

            return CreatedAtAction(nameof(GetById), new { id = created.MauSacId }, MapToDTO(created));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] MauSacDTO dto)
        {
            var entity = new MauSac
            {
                MauSacId = id,
                TenMau = dto.TenMau,
                MoTa = dto.MoTa,
                TrangThai = dto.TrangThai
            };

            var updated = await _mauSacRepo.UpdateAsync(entity);
            if (updated == null) return NotFound();

            // ✅ Ghi thông báo
            await _thongBaoRepo.ThemThongBaoAsync($"✏️ Đã cập nhật màu sắc: {updated.TenMau}");

            return Ok(MapToDTO(updated));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _mauSacRepo.DeleteAsync(id);
            if (!deleted) return NotFound();

            // ✅ Ghi thông báo
            await _thongBaoRepo.ThemThongBaoAsync($"🗑️ Đã xoá màu sắc có ID: {id}");

            return NoContent();
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchByTen([FromQuery] string keyword)
        {
            var result = await _mauSacRepo.SearchByTenAsync(keyword);
            return Ok(result.Select(MapToDTO));
        }
    }
}
