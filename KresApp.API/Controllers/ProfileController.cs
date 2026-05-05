using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using KresApp.Application.DTOs;
using KresApp.Application.Services;
using System.Security.Claims;

namespace KresApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly ProfileService _service;

    public ProfileController(ProfileService service)
    {
        _service = service;
    }

    private string GetUserEmail() => User.FindFirstValue(ClaimTypes.Email) ?? "";

    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        var profile = await _service.GetProfileAsync(GetUserEmail());
        return Ok(profile);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
    {
        await _service.UpdateProfileAsync(GetUserEmail(), dto);
        return NoContent();
    }

    [HttpPut("password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        try
        {
            await _service.ChangePasswordAsync(GetUserEmail(), dto);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
