using API.IRepository;
using API.IRepository.Repository;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static API.Models.DTO.BanHang.DatHang;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HoaDonChiTietsController : ControllerBase
    {
        private readonly IChiTietHoaDonRepository _repository;

        public HoaDonChiTietsController(IChiTietHoaDonRepository repository)
        {
            _repository = repository;
        }

        // GET: api/HoaDonChiTiets
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HoaDonChiTiet>>> GetAll()
        {
            var result = await _repository.GetAllHDCTAsync();
            return Ok(result);
        }

        // GET: api/HoaDonChiTiets/ByHdctId/{hdctId}
        [HttpGet("ByHdctId/{hdctId}")]
        public async Task<ActionResult<IEnumerable<HoaDonChiTiet>>> GetByHoaDonChiTietId(Guid hdctId)
        {
            var result = await _repository.GetByHoaDonChiTietIdAsync(hdctId);
            if (result == null) return NotFound();
            return Ok(result);
        }

        // GET: api/HoaDonChiTiets/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<HoaDonChiTiet>> GetById(Guid id)
        {
            var item = await _repository.GetByIdHDCTAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        // GET: api/HoaDonChiTiets/ByHdctAndGiay?hdctId=xxx&giayId=xxx
        [HttpGet("ByHdctAndGiay")]
        public async Task<ActionResult<HoaDonChiTiet>> GetByHoaDonChiTietvaGiay(Guid hdctId, Guid giayId)
        {
            var item = await _repository.GetByHoaDonChiTietvaGiayAsync(hdctId, giayId);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<ActionResult> Create(HoaDonChiTiet model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _repository.Create(model);
            if (!result) return StatusCode(500, "Không thể thêm chi tiết hóa đơn.");

            return Ok(new { message = "Thêm chi tiết hóa đơn thành công." });
        }
     
    }
}
