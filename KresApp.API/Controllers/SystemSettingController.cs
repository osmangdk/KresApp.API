using KresApp.Application.DTOs;
using KresApp.Application.Services;
using KresApp.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KresApp.API.Controllers;

[ApiController]
[Route("api/system-settings")]
public class SystemSettingController : ControllerBase
{
    private readonly SystemSettingService _service;

    public SystemSettingController(SystemSettingService service)
    {
        _service = service;
    }

    [HttpGet("pre-enrollment-status")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPreEnrollmentStatus()
    {
        try
        {
            var isAllowed = await _service.IsPreEnrollmentAllowedAsync();
            return Ok(new { isAllowed });
        }
        catch (Exception ex)
        {
            // Log the error if possible, for now return internal error but with details
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> GetSettings()
    {
        var settings = await _service.GetSettingsAsync();
        return Ok(settings);
    }

    [HttpPut]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> UpdateSettings(UpdateSystemSettingDto settings)
    {
        try
        {
            await _service.UpdateSettingsAsync(settings);
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }
}
