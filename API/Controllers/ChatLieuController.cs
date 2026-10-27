using API.IRepository;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatLieuController : ControllerBase
    {
        private readonly IChatLieuRepository _chatLieuRepository;
        private readonly IThongBaoRepository _thongBaoRepository; // ✅ thêm

        public ChatLieuController(IChatLieuRepository chatLieuRepository, IThongBaoRepository thongBaoRepository)
        {
            _chatLieuRepository = chatLieuRepository;
            _thongBaoRepository = thongBaoRepository;
        }

        // GET: api/ChatLieu
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _chatLieuRepository.GetAllAsync();
            return Ok(list);
        }

        // GET: api/ChatLieu/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var entity = await _chatLieuRepository.GetByIdAsync(id);
            if (entity == null) return NotFound();
            return Ok(entity);
        }

        // POST: api/ChatLieu
        [HttpPost]
        public async Task<IActionResult> Create(ChatLieu chatLieu)
        {
            chatLieu.ChatLieuId = Guid.NewGuid();
            await _chatLieuRepository.AddAsync(chatLieu);

            // ✅ Ghi thông báo
            await _thongBaoRepository.ThemThongBaoAsync($"➕ Đã thêm chất liệu: {chatLieu.TenChatLieu}");

            return Ok();
        }

        // PUT: api/ChatLieu/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, ChatLieu chatLieu)
        {
            var existing = await _chatLieuRepository.GetByIdAsync(id);
            if (existing == null) return NotFound();

            existing.TenChatLieu = chatLieu.TenChatLieu;
            existing.MoTa = chatLieu.MoTa;
            existing.TrangThai = chatLieu.TrangThai;

            await _chatLieuRepository.UpdateAsync(existing);

            // ✅ Ghi thông báo
            await _thongBaoRepository.ThemThongBaoAsync($"✏️ Đã cập nhật chất liệu: {chatLieu.TenChatLieu}");

            return Ok();
        }

        // DELETE: api/ChatLieu/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var entity = await _chatLieuRepository.GetByIdAsync(id);
            if (entity == null) return NotFound();

            await _chatLieuRepository.DeleteAsync(id);

            // ✅ Ghi thông báo
            await _thongBaoRepository.ThemThongBaoAsync($"🗑️ Đã xoá chất liệu: {entity.TenChatLieu}");

            return Ok();
        }
    }
}
