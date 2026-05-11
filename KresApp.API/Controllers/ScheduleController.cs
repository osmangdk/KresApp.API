using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using KresApp.Application.DTOs;
using KresApp.Application.Services;

namespace KresApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ScheduleController : ControllerBase
{
    private readonly ScheduleService _service;
    public ScheduleController(ScheduleService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        return Ok(await _service.GetAllAsync());
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Teacher,SuperAdmin")]
    public async Task<IActionResult> Create([FromBody] CreateScheduleDto dto)
    {
        await _service.CreateAsync(dto);
        return Ok(new { message = "Eklendi." });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Teacher,SuperAdmin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateScheduleDto dto)
    {
        try {
            await _service.UpdateAsync(id, dto);
            return NoContent();
        } catch { return NotFound(); }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
