using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using KresApp.Application.DTOs;
using KresApp.Application.Services;

namespace KresApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DailyReportController : ControllerBase
{
    private readonly DailyReportService _service;

    public DailyReportController(DailyReportService service)
    {
        _service = service;
    }

    [HttpGet("date/{date}")]
    public async Task<IActionResult> GetByDate(string date, [FromQuery] Guid? classId)
    {
        if (!DateOnly.TryParse(date, out var parsedDate)) return BadRequest("Geçersiz format");
        var records = await _service.GetByDateAsync(parsedDate, classId);
        return Ok(records);
    }

    [HttpGet("child/{childId}/date/{date}")]
    public async Task<IActionResult> GetForChild(Guid childId, string date)
    {
        if (!DateOnly.TryParse(date, out var parsedDate)) return BadRequest("Geçersiz format");
        var record = await _service.GetForChildAsync(childId, parsedDate);
        if (record == null) return NotFound();
        return Ok(record);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Teacher,SuperAdmin")]
    public async Task<IActionResult> Create([FromBody] CreateDailyReportDto dto)
    {
        try {
            await _service.CreateAsync(dto);
            return Ok(new { message = "Başarıyla oluşturuldu." });
        } catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Teacher,SuperAdmin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateDailyReportDto dto)
    {
        try {
            await _service.UpdateAsync(id, dto);
            return NoContent();
        } catch { return NotFound(); }
    }
}
