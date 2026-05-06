using KresApp.Application.Services;
using KresApp.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KresApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ParentsController : ControllerBase
{
    private readonly UserService _userService;

    public ParentsController(UserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetParents()
    {
        var parents = await _userService.GetAll(UserRole.Parent);
        return Ok(parents);
    }
}
