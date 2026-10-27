using API.IRepository; // 👈 Thêm using này để dùng _thongBaoRepository
using API.Models.DTO.BanHang;
using Application.DTOs;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,NhanVien,KhachHang")]
    public class KhachHangController : ControllerBase
    {
        private readonly DbContextApp _context;
        private readonly IThongBaoRepository _thongBaoRepository; // 👈 Inject repo

        public KhachHangController(DbContextApp context, IThongBaoRepository thongBaoRepository)
        {
            _context = context;
            _thongBaoRepository = thongBaoRepository;
        }

        // GET: api/KhachHang
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,NhanVien")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<KhachHang>>> GetAllKhachHang()
        {
            return await _context.Set<KhachHang>()
                .Include(k => k.TaiKhoan)
                .Include(k => k.HoaDons)
                .Include(k => k.GioHangs)
                .Include(k => k.DiaChiKhachHangs)
                .ToListAsync();
        }

        // GET: api/KhachHang/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<KhachHang>> GetKhachHang(Guid id)
        {
            if (User.IsInRole("KhachHang") && (!TryGetCurrentCustomerId(out var currentId)||currentId != id))
            {
                return Forbid();
            }

            var khachHang = await _context.Set<KhachHang>()
                .Include(k => k.TaiKhoan)
                .Include(k => k.HoaDons)
                .Include(k => k.GioHangs)
                .Include(k => k.DiaChiKhachHangs)
                .FirstOrDefaultAsync(k => k.KhachHangId == id);

            if (khachHang == null)
            {
                return NotFound();
            }

            return khachHang;
        }

        // GET: api/KhachHang/me
        // Luôn lấy đúng hồ sơ của khách hàng từ JWT,
        // không tin vào ID do trình duyệt gửi lên.
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "KhachHang")]
        [HttpGet("me")]
        public async Task<ActionResult<KhachHangUpdateDTO>> GetMyProfile()
        {
            if (!TryGetCurrentCustomerId(out var customerId))
            {
                return Unauthorized();
            }

            var profile = await _context.KhachHangs
                .AsNoTracking()
                .Where(x => x.KhachHangId == customerId)
                .Select(x => new KhachHangUpdateDTO
                {
                    KhachHangId = x.KhachHangId,
                    HoTen = x.HoTen,
                    Email = x.Email,
                    SoDienThoai = x.SoDienThoai,
                    GioiTinh = x.GioiTinh,
                    NgaySinh = x.NgaySinh
                })
                .FirstOrDefaultAsync();

            return profile == null ? NotFound() : Ok(profile);
        }

        // POST: api/KhachHang
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,NhanVien")]
        [HttpPost]
        public async Task<ActionResult<KhachHang>> CreateKhachHang(KhachHang khachHang)
        {
            khachHang.KhachHangId = Guid.NewGuid();
            khachHang.NgayTao = DateTime.UtcNow;
            khachHang.NgayCapNhatCuoiCung = DateTime.UtcNow;

            _context.KhachHangs.Add(khachHang);
            await _context.SaveChangesAsync();

            // ✅ Ghi thông báo
            await _thongBaoRepository.ThemThongBaoAsync($"👤 Thêm khách hàng mới: {khachHang.HoTen}");

            return CreatedAtAction(nameof(GetKhachHang), new { id = khachHang.KhachHangId }, khachHang);
        }

        // PUT: api/KhachHang/{id}
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,NhanVien")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateKhachHang(Guid id, KhachHang khachHang)
        {
            if (id != khachHang.KhachHangId)
            {
                return BadRequest("ID không trùng khớp.");
            }

            var khachHangExist = await _context.KhachHangs.FindAsync(id);
            if (khachHangExist == null)
            {
                return NotFound();
            }

            // Update thủ công các field
            khachHangExist.HoTen = khachHang.HoTen;
            khachHangExist.Email = khachHang.Email;
            khachHangExist.SoDienThoai = khachHang.SoDienThoai;
            khachHangExist.NgaySinh = khachHang.NgaySinh;
            khachHangExist.TrangThai = khachHang.TrangThai;
            khachHangExist.TaiKhoanId = khachHang.TaiKhoanId;
            khachHangExist.NgayCapNhatCuoiCung = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // ✅ Ghi thông báo
            await _thongBaoRepository.ThemThongBaoAsync($"✏️ Cập nhật khách hàng: {khachHang.HoTen}");

            return NoContent();
        }

        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,NhanVien")]
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<KhachHang>>> SearchKhachHang(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return Ok(await _context.KhachHangs
                    .Include(kh => kh.TaiKhoan)
                    .Include(kh => kh.HoaDons)
                    .Include(kh => kh.GioHangs)
                    .Include(kh => kh.DiaChiKhachHangs)
                    .ToListAsync());
            }

            keyword = keyword.ToLower();

            var result = await _context.KhachHangs
                .Include(kh => kh.TaiKhoan)
                .Include(kh => kh.HoaDons)
                .Include(kh => kh.GioHangs)
                .Include(kh => kh.DiaChiKhachHangs)
                .Where(kh => kh.HoTen.ToLower().Contains(keyword))
                .ToListAsync();

            return Ok(result);
        }

        // Update thông tin khách hàng (profile)
        [HttpPut("update-profile/{id}")]
        public async Task<IActionResult> UpdateProfile(Guid id, KhachHangUpdateDTO dto)
        {
            if (User.IsInRole("KhachHang") && (!TryGetCurrentCustomerId(out var currentId) || currentId != id))
            {
                return Forbid();
            }

            var khachHang = await _context.KhachHangs.FindAsync(id);

            if (khachHang == null)
            {
                return NotFound("Không tìm thấy khách hàng");
            }

            khachHang.HoTen = dto.HoTen;
            khachHang.Email = dto.Email;
            khachHang.SoDienThoai = dto.SoDienThoai;
            khachHang.GioiTinh = dto.GioiTinh;
            khachHang.NgaySinh = dto.NgaySinh;
            khachHang.NgayCapNhatCuoiCung = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // ghi thông báo
            await _thongBaoRepository.ThemThongBaoAsync($"✏️ Khách hàng cập nhật hồ sơ: {khachHang.HoTen}");

            return Ok(khachHang);
        }

        // PUT: api/KhachHang/me
        // Khách hàng chỉ có thể cập nhật hồ sơ gắn với JWT của mình.
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "KhachHang")]
        [HttpPut("me")]
        public async Task<IActionResult> UpdateMyProfile(KhachHangUpdateDTO dto)
        {
            if (!TryGetCurrentCustomerId(out var customerId))
            {
                return Unauthorized();
            }

            var khachHang = await _context.KhachHangs.FindAsync(customerId);
            if (khachHang == null)
            {
                return NotFound("Không tìm thấy khách hàng.");
            }

            khachHang.HoTen = dto.HoTen.Trim();
            khachHang.Email = dto.Email.Trim();
            khachHang.SoDienThoai = dto.SoDienThoai.Trim();
            khachHang.GioiTinh = string.IsNullOrWhiteSpace(dto.GioiTinh)
                ? null
                : dto.GioiTinh.Trim();
            khachHang.NgaySinh = dto.NgaySinh;
            khachHang.NgayCapNhatCuoiCung = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            await _thongBaoRepository.ThemThongBaoAsync(
                $"✏️ Khách hàng cập nhật hồ sơ: {khachHang.HoTen}");

            return Ok(new KhachHangUpdateDTO
            {
                KhachHangId = khachHang.KhachHangId,
                HoTen = khachHang.HoTen,
                Email = khachHang.Email,
                SoDienThoai = khachHang.SoDienThoai,
                GioiTinh = khachHang.GioiTinh,
                NgaySinh = khachHang.NgaySinh
            });
        }

        private bool TryGetCurrentCustomerId(out Guid customerId)
        {
            var idValue =
                User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                ?? User.FindFirstValue("id");

            return Guid.TryParse(idValue, out customerId);
        }

    }
}