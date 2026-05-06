using KresApp.Application.Services;
using KresApp.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KresApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TeachersController : ControllerBase
{
    private readonly UserService _userService;

    public TeachersController(UserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetTeachers()
    {
        var teachers = await _userService.GetAll(UserRole.Teacher);
        return Ok(teachers);
    }
}
