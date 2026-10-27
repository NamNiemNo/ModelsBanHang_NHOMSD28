using API.IRepository;
using API.Models.DTO;
using Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeGiayController : ControllerBase
    {
        private readonly IDeGiayRepository _deGiayRepository;
        private readonly IThongBaoRepository _thongBaoRepository; // ✅ Thêm

        public DeGiayController(IDeGiayRepository deGiayRepository, IThongBaoRepository thongBaoRepository)
        {
            _deGiayRepository = deGiayRepository;
            _thongBaoRepository = thongBaoRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DeGiayDTO>>> GetAll()
        {
            var entities = await _deGiayRepository.GetAllDegiay();

            var dtos = entities.Select(d => new DeGiayDTO
            {
                DeGiayId = d.DeGiayId,
                TenDeGiay = d.TenDeGiay,
                MoTa = d.MoTa,
                TrangThai = d.TrangThai
            });

            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DeGiayDTO>> GetById(Guid id)
        {
            var entity = await _deGiayRepository.GetDeGiay(id);
            if (entity == null)
                return NotFound();

            var dto = new DeGiayDTO
            {
                DeGiayId = entity.DeGiayId,
                TenDeGiay = entity.TenDeGiay,
                MoTa = entity.MoTa,
                TrangThai = entity.TrangThai
            };

            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult> Create(DeGiayDTO dto)
        {
            var entity = new DeGiay
            {
                DeGiayId = Guid.NewGuid(),
                TenDeGiay = dto.TenDeGiay,
                MoTa = dto.MoTa,
                TrangThai = dto.TrangThai
            };

            await _deGiayRepository.CreateDeGiay(entity);

            // ✅ Ghi thông báo
            await _thongBaoRepository.ThemThongBaoAsync($"➕ Đã thêm đế giày: {dto.TenDeGiay}");

            return Ok();
        }

        [HttpPut]
        public async Task<ActionResult> Update(DeGiayDTO dto)
        {
            var entity = await _deGiayRepository.GetDeGiay(dto.DeGiayId);
            if (entity == null)
                return NotFound();

            entity.TenDeGiay = dto.TenDeGiay;
            entity.MoTa = dto.MoTa;
            entity.TrangThai = dto.TrangThai;

            await _deGiayRepository.UpdateDeGiay(entity);

            // ✅ Ghi thông báo
            await _thongBaoRepository.ThemThongBaoAsync($"✏️ Đã cập nhật đế giày: {dto.TenDeGiay}");

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var entity = await _deGiayRepository.GetDeGiay(id);
            if (entity == null)
                return NotFound();

            await _deGiayRepository.DeleteDeGiay(id);

            // ✅ Ghi thông báo
            await _thongBaoRepository.ThemThongBaoAsync($"🗑️ Đã xoá đế giày: {entity.TenDeGiay}");

            return Ok();
        }
    }
}
