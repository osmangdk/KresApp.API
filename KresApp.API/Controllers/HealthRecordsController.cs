using KresApp.Application.DTOs;
using KresApp.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace KresApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class HealthRecordsController : ControllerBase
{
    private readonly ChildHealthService _service;

    public HealthRecordsController(ChildHealthService service) => _service = service;

    [HttpGet("child/{childId}")]
    public async Task<IActionResult> GetHistory(Guid childId)
    {
        var result = await _service.GetHistoryByChildIdAsync(childId);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateChildHealthRecordDto dto)
    {
        await _service.CreateRecordAsync(dto);
        return Ok(new { message = "Sağlık kaydı oluşturuldu." });
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateChildHealthRecordDto dto)
    {
        await _service.UpdateRecordAsync(dto);
        return Ok(new { message = "Sağlık kaydı güncellendi." });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteRecordAsync(id);
        return NoContent();
    }
}
