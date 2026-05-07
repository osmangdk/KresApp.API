using KresApp.Application.DTOs;
using KresApp.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KresApp.API.Controllers;

[ApiController]
[Route("api/access-requests")]
public class AccessRequestController : ControllerBase
{
    private readonly UserAccessRequestService _service;
    private readonly AuthService _authService;

    public AccessRequestController(UserAccessRequestService service, AuthService authService)
    {
        _service = service;
        _authService = authService;
    }

    /// <summary>
    /// LDAP kullanıcısı erişim talebi gönderir.
    /// E-posta ve şifre ile LDAP doğrulaması yapılır, ardından talep kaydedilir.
    /// </summary>
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Create([FromBody] CreateAccessRequestWithCredentialsDto dto)
    {
        try
        {
            // Önce LDAP ile doğrula
            bool isLdap = await _authService.ValidateLdapAsync(dto.Email, dto.Password);
            if (!isLdap)
                return BadRequest(new { message = "LDAP kimlik doğrulaması başarısız. Kurumsal ağa bağlı olduğunuzdan emin olun." });

            await _service.CreateRequestAsync(dto.Email, new CreateAccessRequestDto(dto.Name, dto.Phone ?? string.Empty, dto.Phone));
            return Ok(new { message = "Erişim talebiniz alındı. Yönetici onayından sonra giriş yapabileceksiniz." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Bekleyen talepleri listeler (Admin/SuperAdmin).
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> GetPending()
    {
        var list = await _service.GetPendingAsync();
        return Ok(list);
    }

    /// <summary>
    /// Admin talebi onaylar ve kullanıcı rolünü atar.
    /// </summary>
    [HttpPost("{id}/approve")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> Approve(Guid id, [FromBody] ApproveAccessRequestDto dto)
    {
        try
        {
            await _service.ApproveAsync(id, dto);
            return Ok(new { message = "Kullanıcı başarıyla oluşturuldu ve onaylandı." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Admin talebi reddeder.
    /// </summary>
    [HttpPost("{id}/reject")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> Reject(Guid id, [FromBody] RejectAccessRequestDto dto)
    {
        try
        {
            await _service.RejectAsync(id, dto.AdminNote);
            return Ok(new { message = "Talep reddedildi." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

public record CreateAccessRequestWithCredentialsDto(
    string Email,
    string Password,
    string Name,
    string? Phone
);

public record RejectAccessRequestDto(string? AdminNote);
