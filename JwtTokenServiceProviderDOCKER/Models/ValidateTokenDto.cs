namespace JwtTokenServiceProviderDOCKER.Models
{
    public class ValidateTokenDto
    {
        public bool ValidToken { get; set; }
        public string? UserId { get; set; }
        public string? Role { get; set; }
        public string? Error { get; set; }
    }
}
