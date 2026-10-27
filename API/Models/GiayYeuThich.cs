using Data.Models;

namespace API.Models
{
    public class GiayYeuThich
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid GiayId { get; set; }
        public Guid KhachHangId { get; set; }

        public DateTime NgayYeuThich { get; set; } = DateTime.Now;

        public Giay Giays { get; set; }
        public KhachHang KhachHangs { get; set; }
    }
}
