using API.IRepository;
using API.Models.DTO;
using Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GiamGiaController : ControllerBase
    {
        private readonly IGiamGiaRepository _giamGiaRepository;
        private readonly IGiayDotGiamGiaRepository _giayDotGiamGiaRepository;
        private readonly IThongBaoRepository _thongBaoRepository;

        public GiamGiaController(
            IGiamGiaRepository giamGiaRepository,
            IGiayDotGiamGiaRepository giayDotGiamGiaRepository,
            IThongBaoRepository thongBaoRepository)
        {
            _giamGiaRepository = giamGiaRepository;
            _giayDotGiamGiaRepository = giayDotGiamGiaRepository;
            _thongBaoRepository = thongBaoRepository;
        }

        // GET: api/GiamGia
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GiamGia>>> GetAll()
        {
            var result = await _giamGiaRepository.GetAllAsync();
            return Ok(result);
        }

        // GET: api/GiamGia/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<GiamGia>> GetById(Guid id)
        {
            var result = await _giamGiaRepository.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        // POST: api/GiamGia
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] GiamGiaCreateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (dto.PhanTramKhuyenMai > 60)
                return BadRequest("Phần trăm khuyến mãi không được vượt quá 60%.");

            var giamGia = new GiamGia
            {
                GiamGiaId = dto.GiamGiaId,
                TenGiamGia = dto.TenGiamGia,
                SanPhamKhuyenMai = dto.SanPhamKhuyenMai,
                PhanTramKhuyenMai = dto.PhanTramKhuyenMai,
                NgayBatDau = dto.NgayBatDau,
                NgayKetThuc = dto.NgayKetThuc,
                TrangThai = dto.TrangThai,
                GiayDotGiamGias = dto.GiayChiTietIds.Select(id => new GiayDotGiamGia
                {
                    GiayDotGiamGiaId = Guid.NewGuid(),
                    GiayChiTietId = id,
                    GiamGiaId = dto.GiamGiaId
                }).ToList()
            };

            var created = await _giamGiaRepository.AddAsync(giamGia);

            await _thongBaoRepository.ThemThongBaoAsync(
                $"🎯 Đã tạo đợt giảm giá: **{giamGia.TenGiamGia}** từ {giamGia.NgayBatDau:dd/MM} đến {giamGia.NgayKetThuc:dd/MM}");

            return CreatedAtAction(nameof(GetById), new { id = created.GiamGiaId }, created);
        }


        // PUT: api/GiamGia/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<GiamGia>> Update(Guid id, [FromBody] GiamGia giamGia)
        {
            if (id != giamGia.GiamGiaId)
                return BadRequest("ID không khớp");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (giamGia.PhanTramKhuyenMai > 60)
                return BadRequest("Phần trăm khuyến mãi không được vượt quá 60%.");

            var updated = await _giamGiaRepository.UpdateAsync(giamGia);
            if (updated == null) return NotFound();

            await _thongBaoRepository.ThemThongBaoAsync($"✏️ Đã cập nhật đợt giảm giá: **{giamGia.TenGiamGia}**");

            return Ok(updated);
        }

        // DELETE: api/GiamGia/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var giamGia = await _giamGiaRepository.GetByIdAsync(id);
            if (giamGia == null) return NotFound();

            var deleted = await _giamGiaRepository.DeleteAsync(id);
            if (!deleted) return BadRequest();

            await _thongBaoRepository.ThemThongBaoAsync($"🗑️ Đã xóa đợt giảm giá: **{giamGia.TenGiamGia}**");

            return NoContent();
        }

        // POST: api/GiamGia/{giamGiaId}/giaychitiet/{giayChiTietId}
        [HttpPost("{giamGiaId}/giaychitiet/{giayChiTietId}")]
        public async Task<IActionResult> AddGiayToDotGiamGia(Guid giamGiaId, Guid giayChiTietId)
        {
            var added = await _giamGiaRepository.AddGiayToDotGiamGia(giamGiaId, giayChiTietId);
            if (!added)
                return BadRequest("Không thể thêm Giày Chi Tiết vào đợt giảm giá");

            await _thongBaoRepository.ThemThongBaoAsync($"➕ Đã thêm sản phẩm vào đợt giảm giá ID: `{giamGiaId}`");

            return Ok();
        }

        // DELETE: api/GiamGia/{giamGiaId}/giaychitiet/{giayChiTietId}
        [HttpDelete("{giamGiaId}/giaychitiet/{giayChiTietId}")]
        public async Task<IActionResult> RemoveGiayFromDotGiamGia(Guid giamGiaId, Guid giayChiTietId)
        {
            var removed = await _giamGiaRepository.RemoveGiayFromDotGiamGia(giamGiaId, giayChiTietId);
            if (!removed) return NotFound();

            await _thongBaoRepository.ThemThongBaoAsync($"➖ Đã gỡ sản phẩm khỏi đợt giảm giá ID: `{giamGiaId}`");

            return Ok();
        }
    }
}
