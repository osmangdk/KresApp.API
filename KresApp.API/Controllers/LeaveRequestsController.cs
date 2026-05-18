using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using KresApp.Application.DTOs;
using KresApp.Application.Services;
using System.Security.Claims;

namespace KresApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LeaveRequestsController : ControllerBase
{
    private readonly LeaveRequestService _service;
    public LeaveRequestsController(LeaveRequestService service) => _service = service;

    [HttpGet("child/{childId}")]
    public async Task<IActionResult> GetByChild(Guid childId) 
        => Ok(await _service.GetByChildAsync(childId));

    [HttpGet("pending/class/{classId}")]
    [Authorize(Roles = "Teacher,Admin,SuperAdmin")]
    public async Task<IActionResult> GetPendingByClass(Guid classId) 
        => Ok(await _service.GetPendingByClassAsync(classId));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateLeaveRequestDto dto)
    {
        await _service.CreateAsync(dto);
        return Ok(new { message = "İzin talebi oluşturuldu." });
    }

    [HttpPost("{id}/approve")]
    [Authorize(Roles = "Teacher,Admin,SuperAdmin")]
    public async Task<IActionResult> Approve(Guid id, [FromBody] ApproveLeaveRequestDto dto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _service.ApproveAsync(id, userId, dto.AdminNote);
        return Ok(new { message = "İzin talebi onaylandı." });
    }

    [HttpPost("{id}/reject")]
    [Authorize(Roles = "Teacher,Admin,SuperAdmin")]
    public async Task<IActionResult> Reject(Guid id, [FromBody] ApproveLeaveRequestDto dto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _service.RejectAsync(id, userId, dto.AdminNote);
        return Ok(new { message = "İzin talebi reddedildi." });
    }
}
