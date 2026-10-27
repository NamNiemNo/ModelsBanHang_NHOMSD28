
namespace API.Models.DTO
{
    public class AnhDTO
    {
        public Guid AnhId { get; set; }
        public Guid GiayChiTietId { get; set; }
        public string DuongDan { get; set; } = string.Empty;
        public string TenAnh { get; set; } = string.Empty;
        public bool TrangThai { get; set; }
    }

}
