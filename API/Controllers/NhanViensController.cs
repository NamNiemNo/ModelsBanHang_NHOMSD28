using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Data.Models;
using API.IRepository;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NhanViensController : ControllerBase
    {
        private readonly INhanVienRepository _repository;
        private readonly IThongBaoRepository _thongBaoRepository; // ✅ Thêm

        public NhanViensController(INhanVienRepository context, IThongBaoRepository thongBaoRepository)
        {
            _repository = context;
            _thongBaoRepository = thongBaoRepository;
        }

        // GET: api/NhanViens
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NhanVien>>> GetNhanViens()
        {
            return Ok(await _repository.GetAllNhanVienAsync());
        }

        // GET: api/NhanViens/5
        [HttpGet("{id}")]
        public async Task<ActionResult<NhanVien>> GetNhanVien(Guid id)
        {
            return Ok(await _repository.GetByIdNhanVienAsync(id));
        }

        // PUT: api/NhanViens/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutNhanVien(Guid id, [FromBody] NhanVien nhanVien)
        {
            if (id != nhanVien.NhanVienId)
            {
                return BadRequest("Id không khớp với dữ liệu nhân viên.");
            }

            try
            {
                await _repository.UpdateNhanVienAsync(nhanVien);

                // ✅ Ghi thông báo
                await _thongBaoRepository.ThemThongBaoAsync($"✏️ Cập nhật nhân viên: {nhanVien.HoTen}");

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi cập nhật: {ex.Message}");
            }
        }

        // POST: api/NhanViens
        [HttpPost]
        public async Task<ActionResult<NhanVien>> PostNhanVien([FromBody] NhanVien nhanVien)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _repository.CreateNhanVien(nhanVien);

            // ✅ Ghi thông báo
            await _thongBaoRepository.ThemThongBaoAsync($"👤 Thêm nhân viên mới: {nhanVien.HoTen}");

            return Ok();
        }

        // DELETE: api/NhanViens/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNhanVien(Guid id)
        {
            var nv = await _repository.GetByIdNhanVienAsync(id);
            if (nv == null) return NotFound();

            await _repository.DeleteNhanVienAsync(id);

            // ✅ Ghi thông báo
            await _thongBaoRepository.ThemThongBaoAsync($"🗑️ Đã xoá nhân viên: {nv.HoTen}");

            return Ok();
        }

        [HttpGet("tk/{taikhoanId}")]
        public async Task<ActionResult<NhanVien>> GetNhanVienByTaiKhoanId(Guid taikhoanId)
        {
            var nhanVien = await _repository.GetIdNhanVienTaiKhoan(taikhoanId);
            if (nhanVien == null)
            {
                return NotFound();
            }

            return Ok(nhanVien);
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<NhanVien>>> Search(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return Ok(await _repository.GetAllNhanVienAsync());

            var result = await _repository.SearchNhanVienAsync(keyword);
            return Ok(result);
        }
    }
}
