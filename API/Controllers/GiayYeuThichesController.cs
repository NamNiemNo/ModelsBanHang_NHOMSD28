using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Models;
using Data.Models;
using API.IRepository;
using API.Models.DTO;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GiayYeuThichController : ControllerBase
    {
        private readonly IGiayYeuThichRepository _repository;

        public GiayYeuThichController(IGiayYeuThichRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("khachhang/{khachHangId}")]
        public async Task<IActionResult> GetByKhachHang(Guid khachHangId)
        {
            var list = await _repository.GetByKhachHangIdAsync(khachHangId);
            return Ok(list);
        }

        [HttpPost("toggle")]
        public async Task<IActionResult> ToggleFavorite([FromBody] GiayYeuThichDTO dto)
        {
            try
            {

                var existing = await _repository.GetByGiayAndKhachHangAsync(dto.GiayId, dto.KhachHangId);
                if (existing != null)
                {
                    await _repository.DeleteAsync(existing.Id);
                }
                else
                {
                    var newItem = new GiayYeuThich
                    {
                        GiayId = dto.GiayId,
                        KhachHangId = dto.KhachHangId,
                        NgayYeuThich = DateTime.Now
                    };
                    await _repository.AddAsync(newItem);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi Server: {ex.Message}");
            }
        }

    }
}
