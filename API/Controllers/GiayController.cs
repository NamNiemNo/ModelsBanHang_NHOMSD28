using API.IRepository;
using API.Models.DTO;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GiayController : ControllerBase
    {
        private readonly IGiayRepository _giayRepo;
        private readonly DbContextApp _context;
        public GiayController(IGiayRepository giayRepo, DbContextApp context)
        {
            _giayRepo = giayRepo;
            _context = context;
        }
        [HttpGet("dropdown-options")]
        public async Task<IActionResult> GetDropdownOptions()
        {
            try
            {
                var options = new
                {
                    thuongHieus = await _context.ThuongHieus
                        .Select(th => new { id = th.ThuongHieuId, name = th.TenThuongHieu })
                        .ToListAsync(),
                    chatLieus = await _context.ChatLieus
                        .Select(cl => new { id = cl.ChatLieuId, name = cl.TenChatLieu })
                        .ToListAsync(),
                    theLoaiGiays = await _context.TheLoaiGiays
                        .Select(tl => new { id = tl.TheLoaiGiayId, name = tl.TenTheLoai })
                        .ToListAsync(),
                    deGiays = await _context.DeGiays
                        .Select(dg => new { id = dg.DeGiayId, name = dg.TenDeGiay })
                        .ToListAsync(),
                    kieuDangs = await _context.KieuDangs
                        .Select(kd => new { id = kd.KieuDangId, name = kd.TenKieuDang })
                        .ToListAsync()
                };
                return Ok(options);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi lấy dữ liệu dropdown: {ex.Message}");
            }
        }
        // Helper để map sang GiayDTO
        private GiayDTO MapToDTO(Giay giay)
        {
            return new GiayDTO
            {
                GiayId = giay.GiayId,
                TenGiay = giay.TenGiay,
                TrangThai = giay.TrangThai,
                NgayTao = giay.NgayTao,
                ThuongHieuId = giay.ThuongHieuId,
                ChatLieuId = giay.ChatLieuId,
                TheLoaiGiayId = giay.TheLoaiGiayId,
                DeGiayId = giay.DeGiayId,
                KieuDangId = giay.KieuDangId,
                TenThuongHieu = giay.ThuongHieu?.TenThuongHieu,
                TenChatLieu = giay.ChatLieu?.TenChatLieu,
                TenTheLoaiGiay = giay.TheLoaiGiay?.TenTheLoai,
                TenDeGiay = giay.DeGiay?.TenDeGiay,
                TenKieuDang = giay.KieuDang?.TenKieuDang,

                ChiTietGiays = giay.GiayChiTiets?.Select(ct => new GiayChiTietDTO
                {
                    GiayChiTietId = ct.GiayChiTietId,
                    GiayId = ct.GiayId,
                    MauSacId = ct.MauSacId,
                    KichCoId = ct.KichCoId,
                    TenGiay = ct.Giay?.TenGiay,
                    Gia = ct.Gia,
                    GiaGoc = ct.Gia,
                    SoLuongCon = ct.SoLuongCon,
                    MoTa = ct.MoTa,
                    TrangThai = ct.TrangThai,
                    size = ct.KichCo.size,
                    TenMau = ct.MauSac?.TenMau,
                    AnhGiay = ct.Anhs.FirstOrDefault()?.DuongDan,
                    NgayTao = ct.NgayTao,
                    NgaySua = ct.NgaySua
                }
                ).ToList() ?? new List<GiayChiTietDTO>()
            };
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var giays = await _giayRepo.GetAllAsync();
                var dtoList = giays.Select(MapToDTO).ToList();
                return Ok(dtoList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi lấy giày: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var giay = await _giayRepo.GetByIdAsync(id);
            if (giay == null) return NotFound();

            return Ok(MapToDTO(giay));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] GiayDTO dto)
        {
            var giay = new Giay
            {
                TenGiay = dto.TenGiay,
                TrangThai = dto.TrangThai,
                ThuongHieuId = dto.ThuongHieuId,
                ChatLieuId = dto.ChatLieuId,
                TheLoaiGiayId = dto.TheLoaiGiayId,
                DeGiayId = dto.DeGiayId,
                KieuDangId = dto.KieuDangId,
            };

            var created = await _giayRepo.AddAsync(giay);
            return CreatedAtAction(nameof(GetById), new { id = created.GiayId }, MapToDTO(created));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] GiayDTO dto)
        {
            var giay = new Giay
            {
                GiayId = id,
                TenGiay = dto.TenGiay,
                TrangThai = dto.TrangThai,
                ThuongHieuId = dto.ThuongHieuId,
                ChatLieuId = dto.ChatLieuId,
                TheLoaiGiayId = dto.TheLoaiGiayId,
                DeGiayId = dto.DeGiayId,
                KieuDangId = dto.KieuDangId,
            };

            var updated = await _giayRepo.UpdateAsync(giay);
            if (updated == null) return NotFound();

            return Ok(MapToDTO(updated));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _giayRepo.DeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string keyword)
        {
            var results = await _giayRepo.SearchByTenAsync(keyword);
            return Ok(results.Select(MapToDTO).ToList());
        }


    }
}
