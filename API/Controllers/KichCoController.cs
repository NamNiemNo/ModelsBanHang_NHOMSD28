using API.IRepository;
using API.Models.DTO;
using Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KichCoController : ControllerBase
    {
        private readonly IKichCoRepository _kichCoRepo;

        public KichCoController(IKichCoRepository kichCoRepo)
        {
            _kichCoRepo = kichCoRepo;
        }

        private KichCoDTO MapToDTO(KichCo kc)
        {
            return new KichCoDTO
            {
                KichCoId = kc.KichCoId,
                TenKichCo = kc.TenKichCo,
                size = kc.size,
                CanNang = kc.size * 0.5f // ví dụ tính Cân Nặng, tuỳ theo logic bạn định nghĩa
            };
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _kichCoRepo.GetAllAsync();
            return Ok(result.Select(MapToDTO));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var kc = await _kichCoRepo.GetByIdAsync(id);
            if (kc == null) return NotFound();

            return Ok(MapToDTO(kc));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] KichCoDTO dto)
        {
            var kichCo = new KichCo
            {
                TenKichCo = dto.TenKichCo,
                size = dto.size,
                MoTa = "", // có thể thay bằng giá trị mặc định nếu DTO không có
                TrangThai = true
            };

            var created = await _kichCoRepo.AddAsync(kichCo);
            return CreatedAtAction(nameof(GetById), new { id = created.KichCoId }, MapToDTO(created));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] KichCoDTO dto)
        {
            var kichCo = new KichCo
            {
                KichCoId = id,
                TenKichCo = dto.TenKichCo,
                size = dto.size,
                MoTa = "", // hoặc cập nhật từ DTO nếu mở rộng
                TrangThai = true
            };

            var updated = await _kichCoRepo.UpdateAsync(kichCo);
            if (updated == null) return NotFound();

            return Ok(MapToDTO(updated));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _kichCoRepo.DeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchByTen([FromQuery] string keyword)
        {
            var result = await _kichCoRepo.SearchByTenAsync(keyword);
            return Ok(result.Select(MapToDTO));
        }
    }
}
