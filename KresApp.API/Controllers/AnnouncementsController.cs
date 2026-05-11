using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using KresApp.Application.DTOs;
using KresApp.Application.Services;
using KresApp.Domain.Enums;

namespace KresApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AnnouncementsController : ControllerBase
{
    private readonly AnnouncementService _service;

    public AnnouncementsController(AnnouncementService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] AnnouncementCategory? category)
    {
        var items = await _service.GetAllAsync(category);
        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item == null) return NotFound();
        return Ok(item);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Teacher,SuperAdmin")]
    public async Task<IActionResult> Create([FromBody] CreateAnnouncementDto dto)
    {
        await _service.CreateAsync(dto);
        return Ok(new { message = "Duyuru yayınlandı." });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Teacher,SuperAdmin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateAnnouncementDto dto)
    {
        try {
            await _service.UpdateAsync(id, dto);
            return NoContent();
        } catch { return NotFound(); }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,Teacher,SuperAdmin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
