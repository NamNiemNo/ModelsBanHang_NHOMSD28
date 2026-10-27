namespace API.Models.DTO
{
    public class LoginResponseDto
    {
        public Guid Id { get; set; }

        public bool IsSuccess { get; set; }

        public string Username { get; set; }
            = string.Empty;

        public string Role { get; set; }
            = string.Empty;

        public string Message { get; set; }
            = string.Empty;

        public string Token { get; set; }
            = string.Empty;

        public DateTime? ExpiresAt { get; set; }

        public string HoTen { get; set; }
            = string.Empty;

        public string Email { get; set; }
            = string.Empty;

        public string SoDienThoai { get; set; }
            = string.Empty;
    }
}