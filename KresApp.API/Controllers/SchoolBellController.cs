using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using KresApp.Application.DTOs;
using KresApp.Application.Services;
using System.Security.Claims;

namespace KresApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SchoolBellController : ControllerBase
{
    private readonly SchoolBellService _service;

    public SchoolBellController(SchoolBellService service)
    {
        _service = service;
    }

    private Guid GetUserId() => Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : Guid.Empty;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] DateTime? date)
    {
        return Ok(await _service.GetAllAsync(date));
    }

    [HttpGet("my-requests")]
    public async Task<IActionResult> GetMyRequests()
    {
        return Ok(await _service.GetByParentAsync(GetUserId()));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateSchoolBellRequestDto dto)
    {
        var result = await _service.CreateAsync(GetUserId(), dto);
        return Ok(result);
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, UpdateSchoolBellStatusDto dto)
    {
        await _service.UpdateStatusAsync(id, dto.Status);
        return Ok();
    }
}
