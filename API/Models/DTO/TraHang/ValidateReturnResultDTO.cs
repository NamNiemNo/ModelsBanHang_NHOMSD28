namespace API.Models.DTO.TraHang
{
    public class ValidateReturnResultDTO
    {
        public Guid GiayId { get; set; }
        public string TenSanPham { get; set; }
        public bool CanReturn { get; set; }
        public int SoLuongConLai { get; set; }
        public string LyDoTuChoi { get; set; }
    }
}