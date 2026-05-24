using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using KresApp.Application.DTOs;
using KresApp.Application.Services;

namespace KresApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MeetingRequestsController : ControllerBase
{
    private readonly MeetingRequestService _service;

    public MeetingRequestsController(MeetingRequestService service)
    {
        _service = service;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> GetAll()
        => Ok(await _service.GetAllAsync());

    [HttpGet("parent")]
    [Authorize(Roles = "Parent")]
    public async Task<IActionResult> GetForParent()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return Ok(await _service.GetByParentAsync(userId));
    }

    [HttpGet("teacher")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> GetForTeacher()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return Ok(await _service.GetByTeacherAsync(userId));
    }

    [HttpGet("child/{childId}")]
    public async Task<IActionResult> GetForChild(Guid childId)
        => Ok(await _service.GetByChildAsync(childId));

    [HttpPost]
    [Authorize(Roles = "Parent")]
    public async Task<IActionResult> Create([FromBody] CreateMeetingRequestDto dto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _service.CreateAsync(userId, dto);
        return Ok(new { message = "Görüşme talebi başarıyla oluşturuldu." });
    }

    [HttpPost("{id}/approve")]
    [Authorize(Roles = "Teacher,Admin,SuperAdmin")]
    public async Task<IActionResult> Approve(Guid id, [FromBody] ApproveMeetingRequestDto dto)
    {
        await _service.ApproveAsync(id, dto);
        return Ok(new { message = "Görüşme talebi onaylandı ve planlandı." });
    }

    [HttpPost("{id}/reject")]
    [Authorize(Roles = "Teacher,Admin,SuperAdmin")]
    public async Task<IActionResult> Reject(Guid id, [FromBody] RejectMeetingRequestDto dto)
    {
        await _service.RejectAsync(id, dto);
        return Ok(new { message = "Görüşme talebi reddedildi." });
    }

    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, [FromBody] string? note)
    {
        await _service.CancelAsync(id, note);
        return Ok(new { message = "Görüşme iptal edildi." });
    }

    [HttpPost("{id}/complete")]
    [Authorize(Roles = "Teacher,Admin,SuperAdmin")]
    public async Task<IActionResult> Complete(Guid id, [FromBody] CompleteMeetingRequestDto dto)
    {
        await _service.CompleteAsync(id, dto);
        return Ok(new { message = "Görüşme raporu girilerek tamamlandı." });
    }
}
