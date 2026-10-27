using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class HoaDon
    {
        [Key]
        public Guid HoaDonId { get; set; } = Guid.NewGuid();
        public string TenNguoiNhan { get; set; } = "";

        public string SoDienThoai { get; set; } = "";

        public string DiaChi { get; set; } = "";
        public Guid? NhanVienId { get; set; }
        public Guid? KhachHangId { get; set; }
        public Guid? HinhThucThanhToanId { get; set; }
        public Guid? VoucherId { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;
        public DateTime NgayNhanHang { get; set; }
        public string? LyDoHuy { get; set; }
        public float TongTienSauKhiGiam { get; set; }//Tong tiền sau khi áp dụng voucher và giảm giá (nếu có),va phiship
        public float PhiShip { get; set; }
        public string TrangThai { get; set; }
        public string TrangThaiThanhToan { get; set; } = "";
        public DateTime? ThoiGianHetHan { get; set; }
        public DateTime? TGXacNhan {  get; set; }
        public DateTime? ChoGiaoHang { get; set; }
        public DateTime? DangGiao { get; set; }
        public DateTime? HoanThanh { get; set; }
        public DateTime? NgayHuy { get; set; }
        [MaxLength(200)]
        public string GhiChu { get; set; }

        public Voucher ? voucher { get; set; }
        public NhanVien? nhanVien { get; set; }
        public HinhThucThanhToan? hinhThucThanhToan {  get; set; }
        public KhachHang? khachHang { get; set; }
        public virtual ICollection<HoaDonChiTiet> HoaDonChiTiets { get; set; } = new List<HoaDonChiTiet>();


    }

}
