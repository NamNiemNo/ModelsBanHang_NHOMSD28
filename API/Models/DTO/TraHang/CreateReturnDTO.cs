using System;
using System.Collections.Generic;

namespace API.Models.DTO.TraHang
{
    public class CreateReturnDTO
    {
        public Guid HoaDonId { get; set; }
        public List<ChiTietTraHangDTO> Items { get; set; } = new List<ChiTietTraHangDTO>();
    }
}