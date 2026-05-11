using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using KresApp.Application.DTOs;
using KresApp.Application.Services;

namespace KresApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AttendanceController : ControllerBase
{
    private readonly AttendanceService _service;

    public AttendanceController(AttendanceService service)
    {
        _service = service;
    }

    [HttpGet("date/{date}")]
    public async Task<IActionResult> GetByDate(string date, [FromQuery] Guid? classId)
    {
        if (!DateOnly.TryParse(date, out var parsedDate)) return BadRequest("Geçersiz tarih formatı.");
        var records = await _service.GetByDateAsync(parsedDate, classId);
        return Ok(records);
    }

    [HttpGet("child/{childId}")]
    public async Task<IActionResult> GetByChild(Guid childId, [FromQuery] string? month)
    {
        var records = await _service.GetByChildAsync(childId, month);
        return Ok(records);
    }

    [HttpGet("summary/{date}")]
    public async Task<IActionResult> GetSummary(string date)
    {
        if (!DateOnly.TryParse(date, out var parsedDate)) return BadRequest("Geçersiz tarih formatı.");
        var summary = await _service.GetSummaryAsync(parsedDate);
        return Ok(summary);
    }

    [HttpPost("bulk")]
    [Authorize(Roles = "Admin,Teacher,SuperAdmin")]
    public async Task<IActionResult> BulkCreate([FromBody] CreateBulkAttendanceDto dto)
    {
        await _service.CreateBulkAsync(dto);
        return Ok(new { message = "Yoklamalar başarıyla kaydedildi." });
    }
}
