using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using KresApp.Application.DTOs;
using KresApp.Application.Services;
using KresApp.Domain.Enums;
using System.Security.Claims;

namespace KresApp.API.Controllers;

public class UpdatePaymentStatusRequest
{
    public PaymentStatus Status { get; set; }
}

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly PaymentService _service;

    public PaymentsController(PaymentService service) => _service = service;

    private Guid GetUserId() => Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : Guid.Empty;
    private string GetUserRole() => User.FindFirstValue(ClaimTypes.Role) ?? "";

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] PaymentStatus? status, [FromQuery] int? month, [FromQuery] int? year)
    {
        var role = GetUserRole();
        Guid? parentId = role == "Parent" ? GetUserId() : null;
        var p = await _service.GetAsync(parentId, status, month, year);
        return Ok(p);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var p = await _service.GetByIdAsync(id);
        if (p == null) return NotFound();
        return Ok(p);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreatePaymentDto dto)
    {
        await _service.CreateAsync(dto);
        return Ok(new { message = "Tahakkuk oluşturuldu." });
    }

    [HttpPut("{id}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdatePaymentStatusRequest req) 
    {
        try {
            await _service.UpdateStatusAsync(id, req.Status);
            return NoContent();
        } catch { return NotFound(); }
    }

    [HttpGet("summary")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetSummary()
    {
        return Ok(await _service.GetSummaryAsync());
    }
}
