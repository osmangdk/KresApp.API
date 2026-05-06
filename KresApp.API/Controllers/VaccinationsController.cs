using KresApp.Application.DTOs;
using KresApp.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace KresApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VaccinationsController : ControllerBase
{
    private readonly VaccinationService _service;

    public VaccinationsController(VaccinationService service) => _service = service;

    [HttpGet("child/{childId}")]
    public async Task<IActionResult> GetByChild(Guid childId)
    {
        var result = await _service.GetByChildIdAsync(childId);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateVaccinationDto dto)
    {
        await _service.CreateAsync(dto);
        return Ok(new { message = "Aşı bilgisi kaydedildi." });
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateVaccinationDto dto)
    {
        await _service.UpdateAsync(dto);
        return Ok(new { message = "Aşı bilgisi güncellendi." });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
