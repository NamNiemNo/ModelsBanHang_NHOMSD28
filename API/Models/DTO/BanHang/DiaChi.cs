namespace API.Models.DTO.BanHang
{
    public class DiaChi
    {
        public string Ten { get; set; }
        public List<DiaChi> Con { get; set; } = new();
    }
}
