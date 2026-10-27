using API.IRepository;
using API.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ThongBaoController : ControllerBase
    {
        private readonly IThongBaoRepository _repo;

        public ThongBaoController(IThongBaoRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("moi")]
        public async Task<IActionResult> GetThongBaoMoi()
        {
            var result = await _repo.GetThongBaoMoiAsync();
            var dtoList = result.Select(tb => new ThongBaoDTO
            {
                NoiDung = tb.NoiDung,
                ThoiGian = tb.ThoiGian
            }).ToList();

            return Ok(dtoList);
        }

        [HttpPost]
        public async Task<IActionResult> TaoThongBao([FromBody] string noiDung)
        {
            if (string.IsNullOrWhiteSpace(noiDung))
                return BadRequest("Nội dung không hợp lệ.");

            await _repo.ThemThongBaoAsync(noiDung);
            return Ok();
        }
    }
}
