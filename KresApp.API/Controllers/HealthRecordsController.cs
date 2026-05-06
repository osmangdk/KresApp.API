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
public class HealthRecordsController : ControllerBase
{
    private readonly ChildHealthService _service;
    private readonly ChildService _childService;

    public HealthRecordsController(ChildHealthService service, ChildService childService)
    {
        _service = service;
        _childService = childService;
    }

    private Guid GetUserId() => Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : Guid.Empty;
    private string GetUserRole() => User.FindFirstValue(ClaimTypes.Role) ?? "";

    [HttpGet("child/{childId}")]
    public async Task<IActionResult> GetHistory(Guid childId)
    {
        if (GetUserRole() == "Parent")
        {
            var child = await _childService.GetByIdAsync(childId);
            if (child == null || child.ParentId != GetUserId()) return Forbid();
        }

        var result = await _service.GetHistoryByChildIdAsync(childId);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateChildHealthRecordDto dto)
    {
        if (GetUserRole() == "Parent")
        {
            var child = await _childService.GetByIdAsync(dto.ChildId);
            if (child == null || child.ParentId != GetUserId()) return Forbid();
        }

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
        if (GetUserRole() == "Parent")
        {
            // Kaydın bu çocuğa ait olduğunu kontrol etmeliyiz. 
            // ChildHealthService'e bir sahiplik kontrol metodu eklemek daha iyi olurdu ama hızlıca ChildId üzerinden gidebiliriz.
            var history = await _service.GetHistoryByRecordIdAsync(id); 
            if (history == null || history.ParentId != GetUserId()) return Forbid();
        }

        await _service.DeleteRecordAsync(id);
        return NoContent();
    }
}
