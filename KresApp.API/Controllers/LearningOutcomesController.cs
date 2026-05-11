using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using KresApp.Application.DTOs;
using KresApp.Application.Services;

namespace KresApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LearningOutcomesController : ControllerBase
{
    private readonly LearningOutcomeService _service;
    public LearningOutcomesController(LearningOutcomeService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? year)
        => Ok(await _service.GetAllAsync(year));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var item = await _service.GetByIdAsync(id);
        return item == null ? NotFound() : Ok(item);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Teacher,SuperAdmin")]
    public async Task<IActionResult> Create([FromBody] CreateLearningOutcomeDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return Ok(result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Teacher,SuperAdmin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateLearningOutcomeDto dto)
    {
        try { await _service.UpdateAsync(id, dto); return NoContent(); }
        catch { return NotFound(); }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
