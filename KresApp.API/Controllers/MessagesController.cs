using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using KresApp.Application.DTOs;
using KresApp.Application.Services;
using System.Security.Claims;

namespace KresApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MessagesController : ControllerBase
{
    private readonly MessageService _service;
    public MessagesController(MessageService service) => _service = service;

    private Guid GetUserId() => Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : Guid.Empty;

    [HttpGet("conversations")]
    public async Task<IActionResult> GetConversations()
    {
        return Ok(await _service.GetConversationsAsync(GetUserId()));
    }

    [HttpGet("conversations/{id}")]
    public async Task<IActionResult> GetMessages(Guid id, [FromQuery] int page = 1)
    {
        return Ok(await _service.GetMessagesAsync(id, page));
    }

    [HttpPost("conversations")]
    public async Task<IActionResult> CreateConversation([FromBody] CreateConversationDto dto)
    {
        await _service.CreateConversationAsync(GetUserId(), dto);
        return Ok(new { message = "Sohbet başlatıldı." });
    }

    [HttpPost("conversations/{id}/messages")]
    public async Task<IActionResult> SendMessage(Guid id, [FromBody] CreateMessageDto dto)
    {
        try {
            await _service.SendMessageAsync(id, GetUserId(), dto);
            return Ok(new { message = "Gönderildi." });
        } catch { return NotFound(); }
    }

    [HttpPut("conversations/{id}/read")]
    public async Task<IActionResult> MarkRead(Guid id)
    {
        await _service.MarkAsReadAsync(id, GetUserId());
        return NoContent();
    }
}
