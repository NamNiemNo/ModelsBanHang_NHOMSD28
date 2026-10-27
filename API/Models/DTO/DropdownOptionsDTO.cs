using System.Text.Json.Serialization;

namespace API.Models.DTO
{
    public class DropdownOptionsDTO
    {
        [JsonPropertyName("thuongHieus")]
        public List<DropdownItem> ThuongHieus { get; set; } = new();

        [JsonPropertyName("chatLieus")]
        public List<DropdownItem> ChatLieus { get; set; } = new();

        [JsonPropertyName("theLoaiGiays")]
        public List<DropdownItem> TheLoaiGiays { get; set; } = new();

        [JsonPropertyName("deGiays")]
        public List<DropdownItem> DeGiays { get; set; } = new();

        [JsonPropertyName("kieuDangs")]
        public List<DropdownItem> KieuDangs { get; set; } = new();
    }

    public class DropdownItem
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }
}