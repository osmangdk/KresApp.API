using KresApp.Application.Interfaces;
using KresApp.Application.Services;
using KresApp.Persistence.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace KresApp.API.Controllers;

/// <summary>
/// Kesinleşmiş listedeki veliler için evrak yükleme ve detay bilgi girişi.
/// Sadece AccountStatus == PendingDocuments olan veli kullanıcıları erişebilir.
/// </summary>
[ApiController]
[Route("api/final-registration")]
[Authorize]
public class FinalRegistrationController : ControllerBase
{
    private readonly IFileStorageService _storage;
    private readonly AppDbContext _db;
    private readonly EnrollmentService _enrollmentService;

    public FinalRegistrationController(
        IFileStorageService storage, AppDbContext db, EnrollmentService enrollmentService)
    {
        _storage = storage;
        _db = db;
        _enrollmentService = enrollmentService;
    }

    private Guid GetUserId()
        => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? Guid.Empty.ToString());

    // ── Mevcut başvuru bilgilerini getir ─────────────────────────────────────
    [HttpGet]
    public async Task<IActionResult> GetMyEnrollment()
    {
        var userId = GetUserId();
        var req = await _db.EnrollmentRequests
            .FirstOrDefaultAsync(r => r.CreatedUserId == userId);

        if (req == null) return NotFound(new { message = "Başvuru bulunamadı." });

        return Ok(new
        {
            req.Id, req.Status,
            req.ChildPhotoUrl, req.MotherPhotoUrl, req.FatherPhotoUrl,
            req.IdCardCopyUrl, req.HealthReportUrl, req.VaccinationCardUrl,
            req.CommitmentDocUrl, req.MediaConsentDocUrl, req.InstitutionIdDocUrl,
            req.InfectiousDiseases, req.ChronicDiseases, req.OtherHealthNotes,
            req.Emergency1Name, req.Emergency1Phone, req.Emergency1Relation, req.Emergency1Address,
            req.Emergency2Name, req.Emergency2Phone, req.Emergency2Relation, req.Emergency2Address,
            req.MediaConsent,
        });
    }

    // ── Detay bilgileri güncelle (sağlık, acil durum, izin) ──────────────────
    [HttpPut("details")]
    public async Task<IActionResult> UpdateDetails([FromBody] UpdateFinalDetailsDto dto)
    {
        var userId = GetUserId();
        var req = await _db.EnrollmentRequests.FirstOrDefaultAsync(r => r.CreatedUserId == userId);
        if (req == null) return NotFound(new { message = "Başvuru bulunamadı." });

        req.InfectiousDiseases = dto.InfectiousDiseases;
        req.ChronicDiseases    = dto.ChronicDiseases;
        req.OtherHealthNotes   = dto.OtherHealthNotes;
        req.Emergency1Name     = dto.Emergency1Name;
        req.Emergency1Relation = dto.Emergency1Relation;
        req.Emergency1Phone    = dto.Emergency1Phone;
        req.Emergency1Address  = dto.Emergency1Address;
        req.Emergency2Name     = dto.Emergency2Name;
        req.Emergency2Relation = dto.Emergency2Relation;
        req.Emergency2Phone    = dto.Emergency2Phone;
        req.Emergency2Address  = dto.Emergency2Address;
        req.MediaConsent       = dto.MediaConsent;

        _db.EnrollmentRequests.Update(req);
        await _db.SaveChangesAsync();
        return Ok(new { message = "Bilgiler güncellendi." });
    }

    // ── Dosya yükle ──────────────────────────────────────────────────────────
    [HttpPost("documents/{field}")]
    [RequestSizeLimit(10 * 1024 * 1024)]
    public async Task<IActionResult> UploadDocument(string field, IFormFile file)
    {
        var userId = GetUserId();
        var req = await _db.EnrollmentRequests.FirstOrDefaultAsync(r => r.CreatedUserId == userId);
        if (req == null) return NotFound(new { message = "Başvuru bulunamadı." });

        if (file == null || file.Length == 0)
            return BadRequest(new { message = "Dosya boş olamaz." });

        var allowed = new[] { "image/jpeg", "image/png", "image/webp", "application/pdf" };
        if (!allowed.Contains(file.ContentType.ToLower()))
            return BadRequest(new { message = "Sadece JPG, PNG, WEBP veya PDF yükleyebilirsiniz." });

        var folder = BuildFolder(req);

        await using var stream = file.OpenReadStream();
        var url = await _storage.UploadFileToFolderAsync(stream, folder, file.FileName, file.ContentType);

        switch (field.ToLower())
        {
            case "childphoto":       req.ChildPhotoUrl       = url; break;
            case "motherphoto":      req.MotherPhotoUrl      = url; break;
            case "fatherphoto":      req.FatherPhotoUrl      = url; break;
            case "idcard":           req.IdCardCopyUrl       = url; break;
            case "healthreport":     req.HealthReportUrl     = url; break;
            case "vaccinationcard":  req.VaccinationCardUrl  = url; break;
            case "commitmentdoc":    req.CommitmentDocUrl    = url; break;
            case "mediaconsentdoc":  req.MediaConsentDocUrl  = url; break;
            case "institutionid":    req.InstitutionIdDocUrl = url; break;
            default:
                return BadRequest(new { message = $"Bilinmeyen dosya alanı: {field}" });
        }

        req.FolderPath = folder;
        _db.EnrollmentRequests.Update(req);
        await _db.SaveChangesAsync();

        return Ok(new { url });
    }

    // ── Evrakları tamamla ────────────────────────────────────────────────────
    [HttpPut("submit")]
    public async Task<IActionResult> SubmitDocuments()
    {
        try
        {
            await _enrollmentService.SubmitDocumentsAsync(GetUserId());
            return Ok(new { message = "Evraklarınız teslim edildi. İnceleme sonucunda bilgilendirileceksiniz." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    private static string BuildFolder(Domain.Entities.EnrollmentRequest req)
    {
        var year  = req.CreatedAt.Year.ToString();
        var tc    = string.IsNullOrWhiteSpace(req.ParentTcKimlikNo) ? "noTC" : req.ParentTcKimlikNo.Trim();
        var birth = req.ChildBirthDate.HasValue ? req.ChildBirthDate.Value.ToString("yyyyMMdd") : "noBirth";
        var name  = req.ChildFullName
            .Replace(" ", "").Replace("ı","i").Replace("İ","I")
            .Replace("ş","s").Replace("Ş","S").Replace("ğ","g").Replace("Ğ","G")
            .Replace("ü","u").Replace("Ü","U").Replace("ö","o").Replace("Ö","O")
            .Replace("ç","c").Replace("Ç","C");
        return $"{year}_{tc}_{birth}_{name}";
    }
}

// DTO
public record UpdateFinalDetailsDto(
    string? InfectiousDiseases, string? ChronicDiseases, string? OtherHealthNotes,
    string? Emergency1Name, string? Emergency1Relation, string? Emergency1Phone, string? Emergency1Address,
    string? Emergency2Name, string? Emergency2Relation, string? Emergency2Phone, string? Emergency2Address,
    bool? MediaConsent
);
