namespace API.Models.DTO.Voucher
{
    public class SendVoucherEmailRequest
    {
        public Guid VoucherId { get; set; }
        // true: gửi tất cả khách hàng đang hoạt động; false: chỉ gửi các ID được chọn.
        public bool SendToAllCustomers { get; set; }
        public List<Guid> CustomerIds { get; set; } = new();
    }
}
