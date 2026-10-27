using API.IRepository;
using API.Models.DTO;
using Data.Models;
using Data.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TaiKhoansController : ControllerBase
    {
        private readonly ITaiKhoanRepository _context;
        private readonly INhanVienRepository _nhanVienRepository;
        private readonly IKhachHangRepository _khachHangRepository;
        private readonly IChucVuRepository _chucVuRepository;
        private readonly IThongBaoRepository _thongBaoRepository;
        private readonly IConfiguration _configuration;

        public TaiKhoansController(
            ITaiKhoanRepository context,
            INhanVienRepository nhanVienRepository,
            IChucVuRepository chucVuRepository,
            IThongBaoRepository thongBaoRepository,
            IKhachHangRepository khachHangRepository,
            IConfiguration configuration)
        {
            _context = context;
            _nhanVienRepository = nhanVienRepository;
            _chucVuRepository = chucVuRepository;
            _thongBaoRepository = thongBaoRepository;
            _khachHangRepository = khachHangRepository;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaiKhoan>>>
            GetTaiKhoans()
        {
            return Ok(
                await _context.GetAllTaiKhoanAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaiKhoan>>
            GetTaiKhoan(Guid id)
        {
            return Ok(
                await _context.GetByIdTaiKhoanAsync(id));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutTaiKhoan(
            Guid id,
            TaiKhoan taiKhoan)
        {
            if (id != taiKhoan.TaikhoanId)
            {
                return BadRequest(
                    "ID tài khoản không trùng khớp.");
            }

            await _context.UpdateTaiKhoanAsync(taiKhoan);

            await _thongBaoRepository
                .ThemThongBaoAsync(
                    $"✏️ Đã cập nhật tài khoản: {taiKhoan.Username}");

            return Ok();
        }

        /*
         * Giữ ẩn danh để khách hàng vẫn có thể đăng ký.
         */
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<TaiKhoan>>
            PostTaiKhoan(TaiKhoan taiKhoan)
        {
            await _context.CreateTaiKhoanAsync(taiKhoan);

            await _thongBaoRepository
                .ThemThongBaoAsync(
                    $"👤 Tạo tài khoản mới: {taiKhoan.Username}");

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult>
            DeleteTaiKhoan(Guid id)
        {
            var taiKhoan =
                await _context.GetByIdTaiKhoanAsync(id);

            if (taiKhoan == null)
            {
                return NotFound();
            }

            await _context.DeleteTaiKhoanAsync(id);

            await _thongBaoRepository
                .ThemThongBaoAsync(
                    $"🗑️ Đã xoá tài khoản: {taiKhoan.Username}");

            return Ok();
        }

        /*
         * API kiểm tra tên đăng nhập khi đăng ký.
         */
        [HttpGet("username/{username}")]
        [AllowAnonymous]
        public async Task<ActionResult<TaiKhoan>>
            GetTaiKhoanByUsername(string username)
        {
            var taiKhoan =
                await _context.GetByUsernameAsync(username);

            if (taiKhoan == null)
            {
                return NotFound();
            }

            return Ok(taiKhoan);
        }

        /*
         * Đăng nhập nhân viên và Admin.
         * Sau khi đúng tài khoản, API tạo JWT có role.
         */
        [HttpGet("login")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResponseDto>>
            Login(
                string username,
                string pass)
        {
            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(pass))
            {
                return BadRequest(
                    new LoginResponseDto
                    {
                        IsSuccess = false,
                        Message =
                            "Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu."
                    });
            }

            var taiKhoan =
                await _context.GetByIdChucVuAsync(
                    username,
                    pass);

            if (taiKhoan?.NhanVien == null)
            {
                return Unauthorized(
                    new LoginResponseDto
                    {
                        IsSuccess = false,
                        Username = username,
                        Message =
                            "Tên đăng nhập hoặc mật khẩu không đúng."
                    });
            }

            if (!taiKhoan.NhanVien.TrangThai)
            {
                return Unauthorized(
                    new LoginResponseDto
                    {
                        IsSuccess = false,
                        Username = username,
                        Message =
                            "Tài khoản đã bị ngừng hoạt động."
                    });
            }

            var nhanVien =
                await _nhanVienRepository
                    .GetIdNhanVienTaiKhoan(
                        taiKhoan.TaikhoanId);

            if (nhanVien?.ChucVuId == null)
            {
                return Unauthorized(
                    new LoginResponseDto
                    {
                        IsSuccess = false,
                        Username = username,
                        Message =
                            "Tài khoản chưa được phân quyền."
                    });
            }

            var chucVu =
                await _chucVuRepository
                    .GetByIdChucVuAsync(
                        nhanVien.ChucVuId.Value);

            var role = chucVu?.TenChucVu;

            if (role != "Admin" &&
                role != "NhanVien")
            {
                return Unauthorized(
                    new LoginResponseDto
                    {
                        IsSuccess = false,
                        Username = username,
                        Message =
                            "Tài khoản không có quyền truy cập trang quản trị."
                    });
            }

            var tokenResult = CreateToken(
                nhanVien.NhanVienId,
                username,
                role);

            return Ok(
                new LoginResponseDto
                {
                    Id = nhanVien.NhanVienId,
                    IsSuccess = true,
                    Username = username,
                    Role = role,
                    Token = tokenResult.Token,
                    ExpiresAt = tokenResult.ExpiresAt,
                    Message = "Đăng nhập thành công!"
                });
        }

        /*
         * Đăng nhập khách hàng.
         * Khách hàng cũng nhận JWT nhưng có role KhachHang.
         */
        [HttpGet("loginkh")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResponseDto>>
            LoginKH(
                string username,
                string pass)
        {
            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(pass))
            {
                return BadRequest(
                    new LoginResponseDto
                    {
                        IsSuccess = false,
                        Message =
                            "Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu."
                    });
            }

            var taiKhoan =
                await _context.GetKhachHang(
                    username,
                    pass);

            if (taiKhoan?.KhachHang == null)
            {
                return Unauthorized(
                    new LoginResponseDto
                    {
                        IsSuccess = false,
                        Username = username,
                        Message =
                            "Tên đăng nhập hoặc mật khẩu không đúng."
                    });
            }

            var khachHang = taiKhoan.KhachHang;

            var tokenResult = CreateToken(
                khachHang.KhachHangId,
                username,
                "KhachHang");

            return Ok(
                new LoginResponseDto
                {
                    Id = khachHang.KhachHangId,
                    IsSuccess = true,
                    Username = username,
                    Role = "KhachHang",
                    Token = tokenResult.Token,
                    ExpiresAt = tokenResult.ExpiresAt,
                    HoTen = khachHang.HoTen,
                    Email = khachHang.Email,
                    SoDienThoai =
                        khachHang.SoDienThoai,
                    Message = "Đăng nhập thành công!"
                });
        }

        /*
         * BlazorAdmin gọi API này mỗi lần tải lại trang
         * để kiểm tra token có thật sự hợp lệ hay không.
         */
        [HttpGet("me")]
        public ActionResult<LoginResponseDto>
            GetCurrentUser()
        {
            var idValue =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue(
                    JwtRegisteredClaimNames.Sub);

            var username =
                User.FindFirstValue(
                    ClaimTypes.Name);

            var role =
                User.FindFirstValue(
                    ClaimTypes.Role);

            if (!Guid.TryParse(idValue, out var id) ||
                string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(role))
            {
                return Unauthorized();
            }

            return Ok(
                new LoginResponseDto
                {
                    Id = id,
                    IsSuccess = true,
                    Username = username,
                    Role = role,
                    Message = "Token hợp lệ."
                });
        }

        private (string Token, DateTime ExpiresAt)
            CreateToken(
                Guid userId,
                string username,
                string role)
        {
            var key =
                _configuration["Jwt:Key"]
                ?? throw new InvalidOperationException(
                    "Chưa cấu hình Jwt:Key.");

            var issuer =
                _configuration["Jwt:Issuer"];

            var audience =
                _configuration["Jwt:Audience"];

            var expiryMinutes =
                _configuration.GetValue<int?>(
                    "Jwt:ExpiryMinutes") ?? 60;

            var expiresAt =
                DateTime.UtcNow.AddMinutes(
                    expiryMinutes);

            var claims = new List<Claim>
            {
                new(
                    JwtRegisteredClaimNames.Sub,
                    userId.ToString()),

                new(
                    ClaimTypes.NameIdentifier,
                    userId.ToString()),

                new(
                    ClaimTypes.Name,
                    username),

                new(
                    ClaimTypes.Role,
                    role),

                new(
                    JwtRegisteredClaimNames.Jti,
                    Guid.NewGuid().ToString())
            };

            var credentials =
                new SigningCredentials(
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(key)),
                    SecurityAlgorithms.HmacSha256);

            var jwt =
                new JwtSecurityToken(
                    issuer: issuer,
                    audience: audience,
                    claims: claims,
                    notBefore: DateTime.UtcNow,
                    expires: expiresAt,
                    signingCredentials: credentials);

            return (
                new JwtSecurityTokenHandler()
                    .WriteToken(jwt),
                expiresAt);
        }
    }
}