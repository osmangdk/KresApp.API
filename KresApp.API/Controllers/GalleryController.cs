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
public class GalleryController : ControllerBase
{
    private readonly GalleryService _service;

    public GalleryController(GalleryService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] Guid? classId, [FromQuery] Guid? childId)
    {
        var result = await _service.GetForUserAsync(classId, childId);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<IActionResult> Create([FromBody] CreateGalleryItemDto dto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _service.CreateAsync(dto, userId);
        return Ok(new { message = "Medya başarıyla eklendi." });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
