using Microsoft.IdentityModel.Tokens;
using JwtTokenServiceProviderDOCKER.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JwtTokenServiceProviderDOCKER.Services;

public class TokenService
{
    private readonly IConfiguration _config;

    public TokenService(IConfiguration config) => _config = config;

    public string GenerateToken(string userId, string role)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Role, role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        int days;
        var configValue = _config["Jwt:ExpiresInDays"];

        if (!int.TryParse(configValue, out days))
        {
            days = 1; // fallback om konfigurationen är felaktig eller saknas
        }

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(days),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public ValidateTokenDto ValidateToken(string token)
    {
        var validationResult = new ValidateTokenDto();

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]!);

        try
        {
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = _config["Jwt:Issuer"],
                ValidAudience = _config["Jwt:Audience"],
                ClockSkew = TimeSpan.Zero
            }, out var validatedToken);

            validationResult.ValidToken = true;
            validationResult.UserId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            validationResult.Role = principal.FindFirst(ClaimTypes.Role)?.Value;
        }
        catch (Exception ex)
        {
            validationResult.ValidToken = false;
            validationResult.Error = ex.Message;
        }

        return validationResult;
    }
}
