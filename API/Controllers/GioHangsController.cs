using API.IRepository;
using API.Models.DTO;
using Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")] // Địa chỉ sẽ là api/GioHangs
    [ApiController]            // Bắt buộc phải có để Swagger hiển thị
    public class GioHangsController : ControllerBase // Dùng ControllerBase cho API
    {
        private readonly IGioHangRepository _giohang;

        public GioHangsController(IGioHangRepository giohang)
        {
            _giohang = giohang;
        }

        [HttpGet("GetAllGioHang")]
        public async Task<IActionResult> GetAllGioHang()
        {
            return Ok(await _giohang.GetAllGioHang());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGioHangByID(Guid id)
        {
            return Ok(await _giohang.GetGioHang(id));
        }

        [HttpPost("Create")]
        // Xóa [ValidateAntiForgeryToken] vì API thường dùng Token/Header thay vì Cookie
        public async Task<IActionResult> Create(GioHang gioHang)
        {
            await _giohang.CreateGioHang(gioHang);
            return Ok();
        }

        [HttpPut("Edit")] // API nên dùng HttpPut cho cập nhật
        public async Task<IActionResult> Edit(GioHang gioHang)
        {
            await _giohang.UpdateGioHang(gioHang);
            return Ok();
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _giohang.DeleteGioHang(id);
            return Ok();
        }


        [HttpGet("khach-hang/{khachHangId}")]
        public async Task<IActionResult> GetGioHangByKhachHang(Guid khachHangId)
        {
            var gioHang = await _giohang.GetGioHangByKhachHang(khachHangId);

            if (gioHang == null)
                return Ok(new GioHangDTO());

            var dto = new GioHangDTO
            {
                DanhSachChiTiet = gioHang.GioHangChiTiets
                 .GroupBy(x => x.GiayChiTietId)
                    .Select(g =>
               {
                     var x = g.First();

                     return new GioHangChiTietDTO
                        {
                            GioHangChiTietId = x.GioHangChiTietId,
                            GiayChiTietId = x.GiayChiTietId,

                            TenGiay = x.GiayChiTiet.Giay.TenGiay,
                            Gia = x.Gia,

                            // 🔥 cộng tổng số lượng
                            SoLuong = g.Sum(i => i.SoLuongSanPham),

                            Size = x.GiayChiTiet.KichCo?.TenKichCo,
                            MauSac = x.GiayChiTiet.MauSac?.TenMau,
                            HinhAnh = x.GiayChiTiet.Anhs.FirstOrDefault()?.DuongDan
                        };
                }).ToList()

            };


            return Ok(dto);
        }

        [HttpPut("cap-nhat-so-luong")]
        public async Task<IActionResult> CapNhatSoLuong(
    Guid id,
    int soLuong)
        {
            try
            {
                await _giohang.CapNhatSoLuong(id, soLuong);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}