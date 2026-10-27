namespace API.Models.DTO
{
    public class AnhUploadDTO
    {
        public Guid GiayChiTietId { get; set; }
        public string TenAnh { get; set; } = string.Empty;
        public IFormFile File { get; set; } = default!;
    }

}
