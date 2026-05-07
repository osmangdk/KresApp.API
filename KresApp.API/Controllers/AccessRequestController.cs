using KresApp.Application.DTOs;
using KresApp.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace KresApp.API.Controllers;

[ApiController]
[Route("api/access-requests")]
public class AccessRequestController : ControllerBase
{
    private readonly UserAccessRequestService _service;
    private readonly AuthService _authService;
    private readonly IMemoryCache _cache;

    public AccessRequestController(UserAccessRequestService service, AuthService authService, IMemoryCache cache)
    {
        _service = service;
        _authService = authService;
        _cache = cache;
    }

    /// <summary>
    /// LDAP kullanıcısı erişim talebi gönderir.
    /// </summary>
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Create([FromBody] CreateAccessRequestWithCredentialsDto dto)
    {
        try
        {
            // 1. Senaryo: Az önce Login ekranından LDAP doğrulaması alıp gelmiş mi?
            if (_cache.TryGetValue($"ldap_verified_{dto.Email}", out bool isVerified) && isVerified)
            {
                await _service.CreateRequestAsync(dto.Email, new CreateAccessRequestDto(dto.Email, dto.Name, dto.Phone));
                // Kullanıldıktan sonra işareti temizleyelim
                _cache.Remove($"ldap_verified_{dto.Email}");
                return Ok(new { message = "Erişim talebiniz alındı. Yönetici onayından sonra giriş yapabileceksiniz." });
            }

            // 2. Senaryo: Doğrudan buradan (veya şifreyle) talep gönderiliyorsa LDAP + SMS doğrulaması yap
            var result = await _authService.ValidateLdapWithMfaAsync(dto.Email, dto.Password ?? "");
            if (result.Success && result.RequiresOtp)
            {
                return Ok(new { status = "requires_otp", email = dto.Email });
            }

            if (!result.Success)
                return BadRequest(new { message = "LDAP bilgileri hatalı veya doğrulama süresi dolmuş." });

            await _service.CreateRequestAsync(dto.Email, new CreateAccessRequestDto(dto.Email, dto.Name, dto.Phone));
            return Ok(new { message = "Erişim talebiniz alındı. Yönetici onayından sonra giriş yapabileceksiniz." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("verify-otp")]
    [AllowAnonymous]
    public async Task<IActionResult> VerifyOtpAndCreate([FromBody] VerifyOtpAndCreateRequestDto dto)
    {
        try
        {
            var isValid = await _authService.VerifyOtpOnly(dto.Email, dto.Code);
            if (!isValid) return BadRequest(new { message = "Doğrulama kodu hatalı veya süresi dolmuş." });

            await _service.CreateRequestAsync(dto.Email, new CreateAccessRequestDto(dto.Email, dto.Name, dto.Phone));
            return Ok(new { message = "Doğrulama başarılı. Erişim talebiniz alındı." });
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
    string? Password,
    string Name,
    string? Phone
);

public record VerifyOtpAndCreateRequestDto(
    string Email,
    string Code,
    string Name,
    string? Phone
);

public record RejectAccessRequestDto(string? AdminNote);
