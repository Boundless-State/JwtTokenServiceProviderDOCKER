namespace JwtTokenServiceProviderDOCKER.Models;

public class CreateTokenDto
{
    public string UserId { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
