using System;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class DiaChiKhachHangDto
    {
        public Guid DiaChiKhachHangId { get; set; }

        [Required]
        [StringLength(200)]
        public string DiaChiCuThe { get; set; }

        [StringLength(100)]
        public string PhuongXa { get; set; }

        [StringLength(100)]
        public string QuanHuyen { get; set; }

        [StringLength(100)]
        public string ThanhPho { get; set; }

        public Guid KhachHangId { get; set; }

        public bool TrangThai { get; set; } = true;

        public bool IsDefault { get; set; } = false;

        public string DiaChiDayDu =>
            $"{DiaChiCuThe}, {PhuongXa}, {QuanHuyen}, {ThanhPho}"
            .Trim(' ', ',');
        // thêm 
        public DateTime NgayTao { get; set; } = DateTime.Now;
        public DateTime NgayCapNhat { get; set; } = DateTime.Now;
    }

}
