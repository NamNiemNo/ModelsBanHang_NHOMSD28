namespace API.Models.DTO
{
    public class GiayYeuThichDTO
    {
        public Guid Id { get; set; }
        public Guid GiayId { get; set; }
        public Guid KhachHangId { get; set; }
        public DateTime NgayYeuThich { get; set; }
    }
}
