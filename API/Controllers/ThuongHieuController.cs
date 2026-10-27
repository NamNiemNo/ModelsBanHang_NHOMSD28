using API.IRepository;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ThuongHieuController : ControllerBase
    {
        private readonly IThuongHieuRepository _thuongHieuRepository;
        private readonly IThongBaoRepository _thongBaoRepository; // ✅ Thêm

        public ThuongHieuController(
            IThuongHieuRepository thuongHieuRepository,
            IThongBaoRepository thongBaoRepository) // ✅ Inject
        {
            _thuongHieuRepository = thuongHieuRepository;
            _thongBaoRepository = thongBaoRepository;
        }

        // GET: api/ThuongHieu
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _thuongHieuRepository.GetAllAsync();
            return Ok(list);
        }

        // GET: api/ThuongHieu/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var entity = await _thuongHieuRepository.GetByIdAsync(id);
            if (entity == null) return NotFound();
            return Ok(entity);
        }

        // POST: api/ThuongHieu
        [HttpPost]
        public async Task<IActionResult> Create(ThuongHieu thuongHieu)
        {
            thuongHieu.ThuongHieuId = Guid.NewGuid();
            await _thuongHieuRepository.AddAsync(thuongHieu);

            // ✅ Ghi thông báo
            await _thongBaoRepository.ThemThongBaoAsync($"🏷️ Thêm thương hiệu mới: {thuongHieu.TenThuongHieu}");

            return Ok();
        }

        // PUT: api/ThuongHieu/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, ThuongHieu thuongHieu)
        {
            var existing = await _thuongHieuRepository.GetByIdAsync(id);
            if (existing == null) return NotFound();

            existing.TenThuongHieu = thuongHieu.TenThuongHieu;
            existing.Email = thuongHieu.Email;
            existing.SDT = thuongHieu.SDT;
            existing.DiaChi = thuongHieu.DiaChi;
            existing.MoTa = thuongHieu.MoTa;
            existing.TrangThai = thuongHieu.TrangThai;

            await _thuongHieuRepository.UpdateAsync(existing);

            // ✅ Ghi thông báo
            await _thongBaoRepository.ThemThongBaoAsync($"✏️ Cập nhật thương hiệu: {thuongHieu.TenThuongHieu}");

            return Ok();
        }

        // DELETE: api/ThuongHieu/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var entity = await _thuongHieuRepository.GetByIdAsync(id);
            if (entity == null) return NotFound();

            await _thuongHieuRepository.DeleteAsync(id);

            // ✅ Ghi thông báo
            await _thongBaoRepository.ThemThongBaoAsync($"🗑️ Đã xoá thương hiệu: {entity.TenThuongHieu}");

            return Ok();
        }
    }
}
