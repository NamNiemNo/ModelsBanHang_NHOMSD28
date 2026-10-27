using API.IRepository;
using API.Models.DTO;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GiayChiTietController : ControllerBase
    {
        private readonly IGiayChiTietRepository _repo;
        private readonly DbContextApp _context;

        public GiayChiTietController(IGiayChiTietRepository repo, DbContextApp context)
        {
            _repo = repo;
            _context = context;
        }

        private (float giaSauGiam, bool daGiam) TinhGiaSauGiam(GiayChiTiet entity)
        {
            var now = DateTime.Now;

            var dot = _context.GiamGias
                .Include(g => g.GiayDotGiamGias)
                .FirstOrDefault(g =>
                    g.TrangThai &&
                    g.NgayBatDau <= now &&
                    g.NgayKetThuc >= now &&
                  g.GiayDotGiamGias.Any(x => x.GiayChiTietId == entity.GiayChiTietId));


            if (dot != null)
            {
                var giaSau = (float)Math.Round(entity.Gia * (1 - dot.PhanTramKhuyenMai / 100f), 0);
                return (giaSau, true);
            }

            return (entity.Gia, false);
        }

        private GiayChiTietDTO MapToDTO(GiayChiTiet entity)
        {
            var (giaSauGiam, daGiamGia) = TinhGiaSauGiam(entity);

            return new GiayChiTietDTO
            {
                GiayChiTietId = entity.GiayChiTietId,
                GiayId = entity.GiayId,
                KichCoId = entity.KichCoId,
                MauSacId = entity.MauSacId,
                TenGiay = entity.Giay?.TenGiay,
                TenMau = entity.MauSac?.TenMau,
                size = entity.KichCo?.size ?? 0,
                Gia = giaSauGiam,
                GiaGoc = entity.Gia,
                DaGiamGia = daGiamGia,
                SoLuongCon = entity.SoLuongCon,
                MoTa = entity.MoTa,
                TrangThai = entity.TrangThai,
                AnhGiay = entity.Anhs.FirstOrDefault()?.DuongDan,
                NgayTao = entity.NgayTao,
                NgaySua = entity.NgaySua
            };
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GiayChiTietDTO>>> GetAll()
        {
            var entities = await _repo.GetAllAsync();
            var dtos = entities.Select(MapToDTO);
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return NotFound();
            return Ok(MapToDTO(entity));
        }

        [HttpGet("giay/{giayId}")]
        public async Task<IActionResult> GetByGiayId(Guid giayId)
        {
            var entities = await _repo.GetByGiayIdAsync(giayId);
            return Ok(entities.Select(MapToDTO));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] GiayChiTietDTO dto)
        {
            if (dto.GiayId == Guid.Empty)
                return BadRequest("GiayId là bắt buộc.");

            var entity = new GiayChiTiet
            {
                GiayId = dto.GiayId,
                KichCoId = dto.KichCoId,
                MauSacId = dto.MauSacId,
                Gia = dto.Gia,
                SoLuongCon = dto.SoLuongCon,
                MoTa = dto.MoTa,
                TrangThai = dto.TrangThai,
                NgayTao = DateTime.UtcNow,
                NgaySua = DateTime.UtcNow
            };

            var created = await _repo.AddAsync(entity);
            return CreatedAtAction(nameof(GetById), new { id = created.GiayChiTietId }, MapToDTO(created));
        }

        [HttpPost("multiple")]
        public async Task<IActionResult> CreateMultiple([FromBody] List<GiayChiTietDTO> dtos)
        {
            if (dtos == null || !dtos.Any())
                return BadRequest("Danh sách không được rỗng.");

            var entities = dtos.Select(dto => new GiayChiTiet
            {
                GiayId = dto.GiayId,
                KichCoId = dto.KichCoId,
                MauSacId = dto.MauSacId,
                Gia = dto.Gia,
                SoLuongCon = dto.SoLuongCon,
                MoTa = dto.MoTa,
                TrangThai = dto.TrangThai,
                NgayTao = DateTime.UtcNow,
                NgaySua = DateTime.UtcNow
            }).ToList();

            var createdList = await _repo.AddMultipleReturnAsync(entities);
            return Ok(createdList.Select(MapToDTO));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] GiayChiTietDTO dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return NotFound();

            existing.KichCoId = dto.KichCoId;
            existing.MauSacId = dto.MauSacId;
            existing.Gia = dto.Gia;
            existing.SoLuongCon = dto.SoLuongCon;
            existing.MoTa = dto.MoTa;
            existing.TrangThai = dto.TrangThai;
            existing.NgaySua = DateTime.UtcNow;

            var updated = await _repo.UpdateAsync(existing);
            return Ok(MapToDTO(updated));
        }
    }
}
