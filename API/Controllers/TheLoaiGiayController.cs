using API.Models.DTO;
using Data.IRepository;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using API.IRepository; // ✅ Thêm using
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TheLoaiGiayController : ControllerBase
    {
        private readonly ITheLoaiGiayRepository _repository;
        private readonly IThongBaoRepository _thongBaoRepository; // ✅ Thêm
        private readonly ILogger<TheLoaiGiayController> _logger;

        public TheLoaiGiayController(
            ITheLoaiGiayRepository repository,
            IThongBaoRepository thongBaoRepository,
            ILogger<TheLoaiGiayController> logger)
        {
            _repository = repository;
            _thongBaoRepository = thongBaoRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TheLoaiGiayDTO>>> GetAll()
        {
            var result = await _repository.GetAllAsync();
            var dtos = result.Select(t => new TheLoaiGiayDTO
            {
                TheLoaiGiayId = t.TheLoaiGiayId,
                TenTheLoai = t.TenTheLoai,
                MoTa = t.MoTa,
                TrangThai = t.TrangThai
            });
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TheLoaiGiayDTO>> GetById(Guid id)
        {
            var item = await _repository.GetByIdAsync(id);
            if (item == null) return NotFound();

            var dto = new TheLoaiGiayDTO
            {
                TheLoaiGiayId = item.TheLoaiGiayId,
                TenTheLoai = item.TenTheLoai,
                MoTa = item.MoTa,
                TrangThai = item.TrangThai
            };
            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<TheLoaiGiayDTO>> Create(TheLoaiGiayDTO dto)
        {
            var model = new TheLoaiGiay
            {
                TheLoaiGiayId = Guid.NewGuid(),
                TenTheLoai = dto.TenTheLoai,
                MoTa = dto.MoTa,
                TrangThai = dto.TrangThai
            };

            var created = await _repository.AddAsync(model);

            // ✅ Ghi thông báo
            await _thongBaoRepository.ThemThongBaoAsync($"👟 Thêm thể loại giày mới: {created.TenTheLoai}");

            dto.TheLoaiGiayId = created.TheLoaiGiayId;
            return CreatedAtAction(nameof(GetById), new { id = dto.TheLoaiGiayId }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, TheLoaiGiayDTO dto)
        {
            if (id != dto.TheLoaiGiayId) return BadRequest("ID không khớp");

            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return NotFound();

            existing.TenTheLoai = dto.TenTheLoai;
            existing.MoTa = dto.MoTa;
            existing.TrangThai = dto.TrangThai;

            var updated = await _repository.UpdateAsync(existing);

            // ✅ Ghi thông báo
            await _thongBaoRepository.ThemThongBaoAsync($"✏️ Cập nhật thể loại giày: {updated.TenTheLoai}");

            return Ok(new TheLoaiGiayDTO
            {
                TheLoaiGiayId = updated.TheLoaiGiayId,
                TenTheLoai = updated.TenTheLoai,
                MoTa = updated.MoTa,
                TrangThai = updated.TrangThai
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return NotFound();

            var result = await _repository.DeleteAsync(id);
            if (!result) return BadRequest();

            // ✅ Ghi thông báo
            await _thongBaoRepository.ThemThongBaoAsync($"🗑️ Đã xoá thể loại giày: {entity.TenTheLoai}");

            return NoContent();
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<TheLoaiGiayDTO>>> SearchByName([FromQuery] string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword)) return BadRequest("Từ khoá trống");

            var result = await _repository.GetAllAsync();
            var matched = result.Where(t => t.TenTheLoai.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                                 .Select(t => new TheLoaiGiayDTO
                                 {
                                     TheLoaiGiayId = t.TheLoaiGiayId,
                                     TenTheLoai = t.TenTheLoai,
                                     MoTa = t.MoTa,
                                     TrangThai = t.TrangThai
                                 })
                                 .ToList();

            if (!matched.Any()) return NotFound();
            return Ok(matched);
        }
    }
}
