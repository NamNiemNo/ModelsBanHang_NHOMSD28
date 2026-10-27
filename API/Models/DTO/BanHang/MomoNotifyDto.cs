namespace API.Models.DTO.BanHang
{
    public class MomoNotifyDto
    {
        public string? OrderId { get; set; }
        public string? RequestId { get; set; }
        public string? ExtraData { get; set; }
        public long Amount { get; set; }
        public int ResultCode { get; set; }
        public string? Message { get; set; }
        public string? TransId { get; set; }
    }
}
