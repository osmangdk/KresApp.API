using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using KresApp.Application.Services;
using System.Security.Claims;

namespace KresApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly DashboardService _service;
    public DashboardController(DashboardService service) => _service = service;

    private Guid GetUserId() => Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : Guid.Empty;
    private string GetUserRole() => User.FindFirstValue(ClaimTypes.Role) ?? "";

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        return Ok(await _service.GetStatsAsync(GetUserId(), GetUserRole()));
    }
}
