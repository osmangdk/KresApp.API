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
        try
        {
            await _auth.Register(dto.Email, dto.Password, dto.Role, dto.Name, dto.Phone);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        try
        {
            var result = await _auth.Login(dto.Email, dto.Password);

            if (result.Status == "not_registered")
            {
                // LDAP doğrulandı ama sistemde hesap yok → Flutter talep ekranına yönlenecek
                return Accepted(new
                {
                    status = "not_registered",
                    ldapName = result.LdapName,
                    email = dto.Email
                });
            }

            return Ok(new { token = result.Token });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
 