using KresApp.Application.Interfaces;
using KresApp.Persistence.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KresApp.API.Controllers;

/// <summary>
/// Başvuru belgelerini MinIO'ya yükler.
/// Klasör adı: {yıl}_{tcNo}_{doğumTarih}_{çocukAdSoyad}
/// Örnek: 2025_12345678901_20220616_BegumGedik
/// </summary>
[ApiController]
[Route("api/enrollment/{enrollmentId:guid}/files")]
[AllowAnonymous] // Başvuru sırasında token yok; enrollmentId gizli GUID yeterince güvenli
public class EnrollmentFileController : ControllerBase
{
    private readonly IFileStorageService _storage;
    private readonly AppDbContext _db;

    public EnrollmentFileController(IFileStorageService storage, AppDbContext db)
    {
        _storage = storage;
        _db = db;
    }

    // ── Tek dosya yükle ────────────────────────────────────────────────────────
    // field: childPhoto | motherPhoto | fatherPhoto | idCard | healthReport |
    //         vaccinationCard | commitmentDoc | mediaConsentDoc | institutionId
    [HttpPost("{field}")]
    [RequestSizeLimit(10 * 1024 * 1024)] // 10 MB
    public async Task<IActionResult> Upload(Guid enrollmentId, string field, IFormFile file)
    {
        var req = await _db.EnrollmentRequests.FindAsync(enrollmentId);
        if (req == null) return NotFound(new { message = "Başvuru bulunamadı." });

        if (file == null || file.Length == 0)
            return BadRequest(new { message = "Dosya boş olamaz." });

        // Sadece izin verilen türler
        var allowed = new[] { "image/jpeg", "image/png", "image/webp", "application/pdf" };
        if (!allowed.Contains(file.ContentType.ToLower()))
            return BadRequest(new { message = "Sadece JPG, PNG, WEBP veya PDF yükleyebilirsiniz." });

        // Klasör adını oluştur: yıl_tcNo_dogumTarih_cocukAd
        var folder = BuildFolder(req);

        await using var stream = file.OpenReadStream();
        var url = await _storage.UploadFileToFolderAsync(stream, folder, file.FileName, file.ContentType);

        // İlgili alana URL'i kaydet
        switch (field.ToLower())
        {
            case "childphoto":       req.ChildPhotoUrl       = url; break;
            case "motherpho":
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

    // ── Klasör adı oluştur ────────────────────────────────────────────────────
    private static string BuildFolder(Domain.Entities.EnrollmentRequest req)
    {
        var year  = req.CreatedAt.Year.ToString();
        var tc    = string.IsNullOrWhiteSpace(req.ParentTcKimlikNo)
                        ? "noTC"
                        : req.ParentTcKimlikNo.Trim();
        var birth = req.ChildBirthDate.HasValue
                        ? req.ChildBirthDate.Value.ToString("yyyyMMdd")
                        : "noBirth";
        var name  = req.ChildFullName
                        .Replace(" ", "")
                        .Replace("ı","i").Replace("İ","I")
                        .Replace("ş","s").Replace("Ş","S")
                        .Replace("ğ","g").Replace("Ğ","G")
                        .Replace("ü","u").Replace("Ü","U")
                        .Replace("ö","o").Replace("Ö","O")
                        .Replace("ç","c").Replace("Ç","C");

        return $"{year}_{tc}_{birth}_{name}";
    }
}
