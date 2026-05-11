using KresApp.Application.DTOs;
using KresApp.Application.Services;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Roles = "Admin,SuperAdmin")]
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

            if (result.Status == "requires_otp")
            {
                return Ok(new { status = "requires_otp", email = dto.Email });
            }

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

            return Ok(new { token = result.Token, accountStatus = result.AccountStatus });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp(VerifyOtpDto dto)
    {
        try
        {
            var token = await _auth.VerifyOtp(dto.Email, dto.Code);
            return Ok(new { token });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
 