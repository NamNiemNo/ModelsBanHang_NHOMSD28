using Application.DTOs;
using Data.IRepository;
using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,NhanVien,KhachHang")]
    public class DiaChiKhachHangController : ControllerBase
    {
        private readonly IDiaChiKhachHangRepository _repository;

        public DiaChiKhachHangController(
            IDiaChiKhachHangRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DiaChiKhachHangDto>>>
            GetAll()
        {
            if (User.IsInRole("KhachHang"))
            {
                return Forbid();
            }

            var data = await _repository.GetAllAsync();

            return Ok(data.Select(MapToDto));
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<DiaChiKhachHangDto>>
            GetById(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);

            if (entity == null)
            {
                return NotFound();
            }

            if (!CanAccess(entity.KhachHangId))
            {
                return Forbid();
            }

            return Ok(MapToDto(entity));
        }

        [HttpGet("khachhang/{khachHangId:guid}")]
        public async Task<ActionResult<IEnumerable<DiaChiKhachHangDto>>>
            GetByKhachHangId(Guid khachHangId)
        {
            if (!CanAccess(khachHangId))
            {
                return Forbid();
            }

            var addresses =
                await _repository.GetByKhachHangIdAsync(khachHangId);

            return Ok(addresses.Select(MapToDto));
        }

        [HttpGet("default/{khachHangId:guid}")]
        public async Task<ActionResult<DiaChiKhachHangDto>>
            GetDefault(Guid khachHangId)
        {
            if (!CanAccess(khachHangId))
            {
                return Forbid();
            }

            var entity =
                await _repository.GetDefaultByKhachHangIdAsync(
                    khachHangId);

            if (entity == null)
            {
                return NotFound();
            }

            return Ok(MapToDto(entity));
        }

        [HttpPost]
        public async Task<IActionResult>
            Create([FromBody] DiaChiKhachHangDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Không tin vào KhachHangId từ trình duyệt.
            // Lấy ID trực tiếp từ JWT.
            if (User.IsInRole("KhachHang"))
            {
                if (!TryGetCurrentCustomerId(out var customerId))
                {
                    return Unauthorized();
                }

                dto.KhachHangId = customerId;
            }

            var entity = MapToEntity(dto);

            entity.DiaChiKhachHangId = Guid.NewGuid();
            entity.TrangThai = true;
            entity.NgayTao = DateTime.Now;
            entity.NgayCapNhat = DateTime.Now;

            var result = await _repository.CreateAsync(entity);

            if (!result)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Không thể thêm địa chỉ.");
            }

            return Ok(new
            {
                message = "Thêm địa chỉ thành công.",
                id = entity.DiaChiKhachHangId
            });
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult>
            Update(
                Guid id,
                [FromBody] DiaChiKhachHangDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var entity = await _repository.GetByIdAsync(id);

            if (entity == null)
            {
                return NotFound("Không tìm thấy địa chỉ.");
            }

            if (!CanAccess(entity.KhachHangId))
            {
                return Forbid();
            }

            entity.DiaChiCuThe = dto.DiaChiCuThe.Trim();
            entity.PhuongXa = dto.PhuongXa?.Trim() ?? string.Empty;
            entity.QuanHuyen = dto.QuanHuyen?.Trim() ?? string.Empty;
            entity.ThanhPho = dto.ThanhPho?.Trim() ?? string.Empty;
            entity.IsDefault = dto.IsDefault;
            entity.TrangThai = true;
            entity.NgayCapNhat = DateTime.Now;

            var result = await _repository.UpdateAsync(entity);

            if (!result)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Không thể cập nhật địa chỉ.");
            }

            return Ok(new
            {
                message = "Cập nhật địa chỉ thành công."
            });
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);

            if (entity == null)
            {
                return NotFound("Không tìm thấy địa chỉ.");
            }

            if (!CanAccess(entity.KhachHangId))
            {
                return Forbid();
            }

            var result = await _repository.DeleteAsync(id);

            if (!result)
            {
                return BadRequest("Không thể xóa địa chỉ.");
            }

            return Ok(new
            {
                message = "Xóa địa chỉ thành công."
            });
        }

        [HttpPut("set-default/{id:guid}")]
        public async Task<IActionResult>
            SetDefault(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);

            if (entity == null)
            {
                return NotFound("Không tìm thấy địa chỉ.");
            }

            if (!CanAccess(entity.KhachHangId))
            {
                return Forbid();
            }

            var result = await _repository.SetDefaultAsync(id);

            if (!result)
            {
                return BadRequest(
                    "Không thể đặt địa chỉ mặc định.");
            }

            return Ok(new
            {
                message = "Đã đặt làm địa chỉ mặc định."
            });
        }

        private DiaChiKhachHangDto MapToDto(
            DiaChiKhachHang entity)
        {
            return new DiaChiKhachHangDto
            {
                DiaChiKhachHangId =
                    entity.DiaChiKhachHangId,

                DiaChiCuThe = entity.DiaChiCuThe,
                PhuongXa = entity.PhuongXa,
                QuanHuyen = entity.QuanHuyen,
                ThanhPho = entity.ThanhPho,
                KhachHangId = entity.KhachHangId,
                TrangThai = entity.TrangThai,
                IsDefault = entity.IsDefault,
                NgayTao = entity.NgayTao,
                NgayCapNhat = entity.NgayCapNhat
            };
        }

        private DiaChiKhachHang MapToEntity(
            DiaChiKhachHangDto dto)
        {
            return new DiaChiKhachHang
            {
                DiaChiKhachHangId =
                    dto.DiaChiKhachHangId,

                DiaChiCuThe =
                    dto.DiaChiCuThe?.Trim()
                    ?? string.Empty,

                PhuongXa =
                    dto.PhuongXa?.Trim()
                    ?? string.Empty,

                QuanHuyen =
                    dto.QuanHuyen?.Trim()
                    ?? string.Empty,

                ThanhPho =
                    dto.ThanhPho?.Trim()
                    ?? string.Empty,

                KhachHangId = dto.KhachHangId,
                TrangThai = dto.TrangThai,
                IsDefault = dto.IsDefault,
                NgayTao = dto.NgayTao,
                NgayCapNhat = dto.NgayCapNhat
            };
        }

        private bool CanAccess(Guid khachHangId)
        {
            // Admin và nhân viên được phép truy cập.
            if (!User.IsInRole("KhachHang"))
            {
                return true;
            }

            return TryGetCurrentCustomerId(out var currentId)
                && currentId == khachHangId;
        }

        private bool TryGetCurrentCustomerId(
            out Guid customerId)
        {
            var idValue =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue(
                    JwtRegisteredClaimNames.Sub)
                ?? User.FindFirstValue("id");

            return Guid.TryParse(
                idValue,
                out customerId);
        }
    }
}