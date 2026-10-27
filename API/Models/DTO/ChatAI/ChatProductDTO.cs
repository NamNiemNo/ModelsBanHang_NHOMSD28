namespace API.Models.DTO.ChatAI
{
    public class ChatProductDTO
    {
        public Guid GiayId { get; set; }

        public string TenGiay { get; set; }
            = string.Empty;

        public string? ThuongHieu { get; set; }

        public float Gia { get; set; }

        public float GiaCaoNhat { get; set; }

        public int SoLuongCon { get; set; }

        public string? AnhGiay { get; set; }

        public string? TheLoai { get; set; }

        public string? KieuDang { get; set; }

        public string? ChatLieu { get; set; }

        public string? DeGiay { get; set; }

        public string? MoTa { get; set; }

        public List<int> KichCos { get; set; }
            = new List<int>();

        public List<string> MauSacs { get; set; }
            = new List<string>();

        public string DuongDan { get; set; }
            = string.Empty;
    }
}