using System;
using System.ComponentModel.DataAnnotations;

namespace API.Models.DTO
{
    public class GiayChiTietDTO
    {
        public Guid GiayChiTietId { get; set; }
        public Guid GiayId { get; set; }
        public Guid? MauSacId { get; set; }
        public Guid? KichCoId { get; set; }
        public string? TenGiay { get; set; }
        public float Gia { get; set; }          // Giá đã giảm (nếu có)
        public float GiaGoc { get; set; }       // ✅ Giá gốc không đổi
        public bool DaGiamGia { get; set; }     // ✅ Có đang áp dụng giảm giá không?
        public int SoLuongCon { get; set; }
        public string? MoTa { get; set; }
        public bool TrangThai { get; set; }
        public int size { get; set; }
        public string? TenMau { get; set; }
        public string? AnhGiay { get; set; }
        public DateTime NgayTao { get; set; } = DateTime.Now;
        public DateTime NgaySua { get; set; } = DateTime.Now;
        public int SoLuongDat { get; set; }
    }
}