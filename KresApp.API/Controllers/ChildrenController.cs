using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using KresApp.Application.DTOs;
using KresApp.Application.Services;
using System.Security.Claims;

namespace KresApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ChildrenController : ControllerBase
{
    private readonly ChildService _service;

    public ChildrenController(ChildService service)
    {
        _service = service;
    }

    private Guid GetUserId() => Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : Guid.Empty;
    private string GetUserRole() => User.FindFirstValue(ClaimTypes.Role) ?? "";

    [HttpGet]
    public async Task<IActionResult> GetChildren()
    {
        var children = await _service.Get(GetUserId(), GetUserRole());
        return Ok(children);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetChild(Guid id)
    {
        var child = await _service.GetByIdAsync(id);
        if (child == null) return NotFound();
        return Ok(child);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string? q, [FromQuery] Guid? classId)
    {
        var children = await _service.SearchAsync(q, classId);
        return Ok(children);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<IActionResult> AddChild([FromBody] CreateChildDto dto)
    {
        var id = await _service.Create(dto);
        return Ok(new { id, message = "Çocuk başarıyla eklendi." });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<IActionResult> UpdateChild(Guid id, [FromBody] UpdateChildDto dto)
    {
        try {
            await _service.UpdateProfileAsync(id, dto);
            return NoContent();
        } catch { return NotFound(); }
    }

    // ---- Alerji Endpoint'leri (Ayrı Tablo) ----

    /// <summary>
    /// Çocuğun tüm alerjilerini getir
    /// </summary>
    [HttpGet("{id}/allergies")]
    public async Task<IActionResult> GetAllergies(Guid id)
    {
        var allergies = await _service.GetAllergiesAsync(id);
        return Ok(allergies);
    }

    /// <summary>
    /// Çocuğun alerjilerini toplu güncelle (mevcut alerjileri siler, yeniden oluşturur)
    /// </summary>
    [HttpPut("{id}/allergies")]
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<IActionResult> UpdateAllergies(Guid id, [FromBody] UpdateAllergiesDto dto)
    {
        try {
            await _service.UpdateAllergiesAsync(id, dto);
            return NoContent();
        } catch { return NotFound(); }
    }

    /// <summary>
    /// Çocuğa tek bir alerji ekle
    /// </summary>
    [HttpPost("{id}/allergies")]
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<IActionResult> AddAllergy(Guid id, [FromBody] ChildAllergyDto dto)
    {
        try {
            var result = await _service.AddAllergyAsync(id, dto);
            return Ok(result);
        } catch { return NotFound(); }
    }

    /// <summary>
    /// Tek bir alerji kaydını sil
    /// </summary>
    [HttpDelete("{id}/allergies/{allergyId}")]
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<IActionResult> DeleteAllergy(Guid id, Guid allergyId)
    {
        try {
            await _service.DeleteAllergyAsync(allergyId);
            return NoContent();
        } catch { return NotFound(); }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteChild(Guid id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
