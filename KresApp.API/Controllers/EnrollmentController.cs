using KresApp.Application.DTOs;
using KresApp.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KresApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EnrollmentController : ControllerBase
{
    private readonly EnrollmentService _service;

    public EnrollmentController(EnrollmentService service)
        => _service = service;

    private Guid GetUserId()
        => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? Guid.Empty.ToString());

    // ── 1. Ön başvuru (anonim) ───────────────────────────────────────────────
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Submit([FromBody] CreateEnrollmentRequestDto dto)
    {
        try
        {
            var id = await _service.SubmitAsync(dto);
            return Ok(new { id, message = $"Başvurunuz alındı (Sicil: {dto.ParentSicilNo}). İnceleme sonucunda bilgilendirileceksiniz." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // ── 2. Admin: Başvuruları listele ────────────────────────────────────────
    [HttpGet]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> GetAll()
        => Ok(await _service.GetAllAsync());

    [HttpGet("pending")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> GetPending()
        => Ok(await _service.GetPendingAsync());

    [HttpGet("finalized")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> GetFinalized()
        => Ok(await _service.GetFinalizedAsync());

    // ── 3. Admin: Puanlama ───────────────────────────────────────────────────
    [HttpPut("{id:guid}/score")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> Score(Guid id, [FromBody] ScoreEnrollmentDto dto)
    {
        try
        {
            await _service.ScoreAsync(id, dto.Score, dto.Notes, GetUserId());
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // ── 4. Admin: Kesinleştir (User oluştur) ─────────────────────────────────
    [HttpPut("{id:guid}/finalize")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> Finalize(Guid id)
    {
        try
        {
            await _service.FinalizeAsync(id, GetUserId());
            return Ok(new { message = "Başvuru kesinleştirildi. Veli hesabı oluşturuldu." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // ── 5. Admin: Evrak onayı → tam erişim ───────────────────────────────────
    [HttpPut("{id:guid}/approve-docs")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> ApproveDocs(Guid id, [FromBody] ApproveEnrollmentDto dto)
    {
        try
        {
            await _service.ApproveDocumentsAsync(id, dto, GetUserId());
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // ── 6. Admin: Reddet ─────────────────────────────────────────────────────
    [HttpPut("{id:guid}/reject")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> Reject(Guid id, [FromBody] RejectEnrollmentDto dto)
    {
        try
        {
            await _service.RejectAsync(id, dto, GetUserId());
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
