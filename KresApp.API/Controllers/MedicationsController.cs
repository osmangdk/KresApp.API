using KresApp.Application.DTOs;
using KresApp.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace KresApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MedicationsController : ControllerBase
{
    private readonly MedicationService _service;

    public MedicationsController(MedicationService service) => _service = service;

    [HttpGet("child/{childId}")]
    public async Task<IActionResult> GetByChild(Guid childId, [FromQuery] DateTime? date)
    {
        var result = await _service.GetByChildIdAsync(childId, date ?? DateTime.Today);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMedicationDto dto)
    {
        await _service.CreateAsync(dto);
        return Ok(new { message = "İlaç bilgisi kaydedildi." });
    }

    [HttpPost("give")]
    public async Task<IActionResult> Give([FromBody] GiveMedicationDto dto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _service.GiveAsync(dto, userId);
        return Ok(new { message = "İlaç verildi olarak işaretlendi." });
    }

    [HttpPost("{id}/toggle-active")]
    public async Task<IActionResult> ToggleActive(Guid id)
    {
        await _service.ToggleActiveAsync(id);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
