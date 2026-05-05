using KresApp.Application.DTOs;
using KresApp.Application.Services;
using Microsoft.AspNetCore.Mvc; 

namespace KresApp.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _auth;

    public AuthController(AuthService auth)
    {
        _auth = auth;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        await _auth.Register(dto.Email, dto.Password, dto.Role, dto.Name, dto.Phone);
        return Ok();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var token = await _auth.Login(dto.Email, dto.Password);
        return Ok(new { token });
    }
}
 