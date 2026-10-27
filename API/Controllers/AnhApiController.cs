using API.Models.DTO;
using API.IRepository;
using Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnhApiController : ControllerBase
    {
        private readonly IAnhRepository _anhRepository;
        private readonly ILogger<AnhApiController> _logger;

        public AnhApiController(IAnhRepository anhRepository, ILogger<AnhApiController> logger)
        {
            _anhRepository = anhRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AnhDTO>>> GetAll()
        {
            try
            {
                var result = await _anhRepository.GetAllAsync();
                var dtoList = result.Select(anh => new AnhDTO
                {
                    AnhId = anh.AnhId,
                    GiayChiTietId = anh.GiayChiTietId,
                    DuongDan = anh.DuongDan,
                    TenAnh = anh.TenAnh,
                    TrangThai = anh.TrangThai
                }).ToList();

                return Ok(dtoList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách ảnh");
                return StatusCode(500, "Đã xảy ra lỗi khi xử lý yêu cầu");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AnhDTO>> GetById(Guid id)
        {
            try
            {
                var anh = await _anhRepository.GetByIdAsync(id);
                if (anh == null) return NotFound();

                return Ok(new AnhDTO
                {
                    AnhId = anh.AnhId,
                    GiayChiTietId = anh.GiayChiTietId,
                    DuongDan = anh.DuongDan,
                    TenAnh = anh.TenAnh,
                    TrangThai = anh.TrangThai
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi lấy ảnh với ID: {id}");
                return StatusCode(500, "Đã xảy ra lỗi khi xử lý yêu cầu");
            }
        }

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<AnhDTO>> Upload([FromForm] AnhUploadDTO dto)
        {
            try
            {
                if (dto.File == null || dto.File.Length == 0)
                    return BadRequest("File ảnh không hợp lệ.");

                var uploadedAnh = await _anhRepository.UploadAsync(dto.File, dto.TenAnh, dto.GiayChiTietId);

                if (uploadedAnh == null)
                    return BadRequest("Upload thất bại hoặc file không đúng định dạng.");

                return Ok(new AnhDTO
                {
                    AnhId = uploadedAnh.AnhId,
                    GiayChiTietId = uploadedAnh.GiayChiTietId,
                    DuongDan = uploadedAnh.DuongDan,
                    TenAnh = uploadedAnh.TenAnh,
                    TrangThai = uploadedAnh.TrangThai
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi upload ảnh");
                return StatusCode(500, "Đã xảy ra lỗi khi upload ảnh");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<AnhDTO>> Update(Guid id, [FromBody] AnhDTO dto)
        {
            try
            {
                if (id != dto.AnhId)
                    return BadRequest("ID không khớp.");

                var updated = await _anhRepository.UpdateAsync(new Anh
                {
                    AnhId = dto.AnhId,
                    GiayChiTietId = dto.GiayChiTietId,
                    DuongDan = dto.DuongDan,
                    TenAnh = dto.TenAnh,
                    TrangThai = dto.TrangThai
                });

                if (updated == null)
                    return NotFound();

                return Ok(new AnhDTO
                {
                    AnhId = updated.AnhId,
                    GiayChiTietId = updated.GiayChiTietId,
                    DuongDan = updated.DuongDan,
                    TenAnh = updated.TenAnh,
                    TrangThai = updated.TrangThai
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi cập nhật ảnh với ID: {id}");
                return StatusCode(500, "Đã xảy ra lỗi khi cập nhật ảnh");
            }
        }

        [HttpPut("update-file/{id}")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<AnhDTO>> UpdateFile(Guid id, [FromForm] AnhUploadDTO dto)
        {
            try
            {
                if (dto.File == null || dto.File.Length == 0)
                    return BadRequest("File ảnh không hợp lệ.");

                var updated = await _anhRepository.UpdateFileAsync(id, dto.File, dto.TenAnh, dto.GiayChiTietId);

                if (updated == null)
                    return NotFound();

                return Ok(new AnhDTO
                {
                    AnhId = updated.AnhId,
                    GiayChiTietId = updated.GiayChiTietId,
                    DuongDan = updated.DuongDan,
                    TenAnh = updated.TenAnh,
                    TrangThai = updated.TrangThai
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi cập nhật file ảnh với ID: {id}");
                return StatusCode(500, "Đã xảy ra lỗi khi cập nhật file ảnh");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var result = await _anhRepository.DeleteAsync(id);
                if (!result)
                    return NotFound();

                return Ok(new { message = "Xóa ảnh thành công." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi xóa ảnh với ID: {id}");
                return StatusCode(500, "Đã xảy ra lỗi khi xóa ảnh");
            }
        }
    }
}