using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using KresApp.Application.DTOs;
using KresApp.Application.Services;

namespace KresApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MealMenuController : ControllerBase
{
    private readonly MealMenuService _service;
    public MealMenuController(MealMenuService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        return Ok(await _service.GetAllAsync());
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateMealMenuDto dto)
    {
        await _service.CreateAsync(dto);
        return Ok(new { message = "Oluşturuldu." });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateMealMenuDto dto)
    {
        try {
            await _service.UpdateAsync(id, dto);
            return NoContent();
        } catch { return NotFound(); }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
