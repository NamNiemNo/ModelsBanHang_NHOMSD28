using API.IRepository;
using API.Models.DTO;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GioHangChiTietController : ControllerBase
    {
        private readonly IGioHangChiTietRepository _repository;

        public GioHangChiTietController(IGioHangChiTietRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _repository.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var item = await _repository.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }
        [HttpPost("AddToCart")]
        public async Task<IActionResult> AddToCart([FromBody] GioHangChiTiet model)
        {
            if (model.SoLuongSanPham <= 0)
                return BadRequest("Số lượng phải > 0");

            // 1️⃣ LẤY TỒN KHO
            var tonKho = await _repository.GetSoLuongTonKhoAsync(
                model.GiayChiTietId
            );

            if (model.SoLuongSanPham > tonKho)
                return BadRequest($"Chỉ còn {tonKho} sản phẩm trong kho");

            // 2️⃣ TÌM SẢN PHẨM ĐÃ CÓ TRONG GIỎ
            var existingItem = await _repository.GetByGioHangVaGiayChiTietAsync(
                model.GioHangId,
                model.GiayChiTietId
            );

            // 3️⃣ CỘNG DỒN
            if (existingItem != null)
            {
                int tongSoLuong = existingItem.SoLuongSanPham + model.SoLuongSanPham;

                if (tongSoLuong > tonKho)
                    return BadRequest($"Chỉ còn {tonKho} sản phẩm trong kho");

                existingItem.SoLuongSanPham = tongSoLuong;
                existingItem.NgayCapNhat = DateTime.Now;

                await _repository.UpdateAsync(existingItem);
                return Ok(existingItem);
            }

            // 4️⃣ THÊM MỚI
            model.GioHangChiTietId = Guid.NewGuid();
            model.NgayTao = DateTime.Now;
            model.NgayCapNhat = DateTime.Now;
            model.TrangThai = true;

            await _repository.AddAsync(model);
            return Ok(model);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
       Guid id,
       [FromBody] int soLuong)
        {
            if (soLuong < 1)
                return BadRequest("Số lượng phải >= 1");

            var currentItem = await _repository.GetByIdAsync(id);
            if (currentItem == null)
                return NotFound();

            // 🔥 LẤY GIÀY CHI TIẾT THEO GIÀY + GIÁ
            var giayChiTiet = await _repository.GetGiayChiTietByIdAsync(
       currentItem.GiayChiTietId
   );

            if (giayChiTiet == null)
                return BadRequest("Không tìm thấy sản phẩm");

            int tonKho = giayChiTiet.SoLuongCon;

            // ❌ CHẶN VƯỢT TỒN KHO
            if (soLuong > tonKho)
                return BadRequest($"Chỉ còn {tonKho} sản phẩm trong kho");

            currentItem.SoLuongSanPham = soLuong;
            currentItem.NgayCapNhat = DateTime.Now;

            await _repository.UpdateAsync(currentItem);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _repository.DeleteAsync(id);
            return NoContent();
        }
    }
}
