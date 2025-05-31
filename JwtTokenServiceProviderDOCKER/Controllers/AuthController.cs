using JwtTokenServiceProviderDOCKER.Models;
using JwtTokenServiceProviderDOCKER.Services;
using Microsoft.AspNetCore.Mvc;

namespace JwtTokenServiceProviderDOCKER.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly TokenService _tokenService;

    public AuthController(TokenService tokenService)
    {
        _tokenService = tokenService;
    }

    // POST /auth/token
    [HttpPost("token")]
    public IActionResult CreateToken([FromBody] CreateTokenDto dto)
    {
        var token = _tokenService.GenerateToken(dto.UserId, dto.Role);
        return Ok(new { token });
    }

    // POST /auth/verify
    [HttpPost("verify")]
    public IActionResult VerifyToken([FromBody] TokenVerificationDto dto)
    {
        var result = _tokenService.ValidateToken(dto.Token);
        if (!result.ValidToken)
            return BadRequest(new { result.ValidToken, result.Error });

        return Ok(new { result.ValidToken, result.UserId, result.Role });
    }
}
