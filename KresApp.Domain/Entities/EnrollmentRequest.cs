namespace KresApp.Domain.Entities;

public class EnrollmentRequest
{
    public Guid Id { get; set; } = Guid.NewGuid();

    // ── Veli / Anne Bilgileri ─────────────────────────────────────────────────
    public string ParentName { get; set; } = default!;
    public string ParentEmail { get; set; } = default!;
    public string ParentPhone { get; set; } = default!;
    public string? ParentTcKimlikNo { get; set; }
    public string ParentPassword { get; set; } = default!;   // BCrypt hash (submit anında hash'lenir)
    public string? ParentJob { get; set; }
    public string? ParentSicilNo { get; set; }
    public string? ParentUnit { get; set; }
    public string? ParentTitle { get; set; }
    public int? ParentServiceYears { get; set; }
    public string? ParentWorkAddress { get; set; }
    public string? ParentHomeAddress { get; set; }

    // ── Baba Bilgileri ───────────────────────────────────────────────────────
    public string? FatherName { get; set; }
    public string? FatherPhone { get; set; }
    public string? FatherJob { get; set; }
    public string? FatherWorkAddress { get; set; }
    public string? FatherTcKimlikNo { get; set; }

    // ── Eş / Diğer Ebeveyn Bilgileri ──────────────────────────────────────────
    public bool? SpouseIsAlive { get; set; } = true;
    public bool? SpouseIsWorking { get; set; }
    public string? SpouseWorkplace { get; set; }
    public bool? SpouseWorkplaceHasDaycare { get; set; }

    // ── Çocuk Bilgileri ──────────────────────────────────────────────────────
    public string ChildFullName { get; set; } = default!;
    public string? ChildTcKimlikNo { get; set; }
    public DateOnly? ChildBirthDate { get; set; }
    public string? ChildGender { get; set; }
    public string? ChildBloodType { get; set; }
    public string? ChildAllergies { get; set; }

    // ── Sağlık İzleme Formu (JSON string) ────────────────────────────────────
    public string? InfectiousDiseases { get; set; }
    public string? ChronicDiseases { get; set; }
    public string? OtherHealthNotes { get; set; }

    // ── Acil Durum Kişileri ──────────────────────────────────────────────────
    public string? Emergency1Name { get; set; }
    public string? Emergency1Relation { get; set; }
    public string? Emergency1Phone { get; set; }
    public string? Emergency1Address { get; set; }
    public string? Emergency2Name { get; set; }
    public string? Emergency2Relation { get; set; }
    public string? Emergency2Phone { get; set; }
    public string? Emergency2Address { get; set; }

    // ── İzinler ──────────────────────────────────────────────────────────────
    public bool? MediaConsent { get; set; }

    // ── Puanlama (Admin tarafından) ──────────────────────────────────────────
    public int? Score { get; set; }
    public string? ScoringNotes { get; set; }

    // ── MinIO Dosya URL'leri (Kesin Kayıt Evrakları) ─────────────────────────
    public string? FolderPath { get; set; }
    public string? ChildPhotoUrl { get; set; }
    public string? MotherPhotoUrl { get; set; }
    public string? FatherPhotoUrl { get; set; }
    public string? IdCardCopyUrl { get; set; }
    public string? HealthReportUrl { get; set; }
    public string? VaccinationCardUrl { get; set; }
    public string? CommitmentDocUrl { get; set; }
    public string? MediaConsentDocUrl { get; set; }
    public string? InstitutionIdDocUrl { get; set; }

    // ── Başvuru Durumu ────────────────────────────────────────────────────────
    public string? Notes { get; set; }
    public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Pending;
    public string? AdminNote { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ReviewedAt { get; set; }
    public Guid? ReviewedByUserId { get; set; }
    public Guid? CreatedUserId { get; set; }       // Finalize sırasında oluşturulan User.Id
}

public enum EnrollmentStatus
{
    Pending            = 0,   // Ön başvuru yapıldı
    Scored             = 1,   // Admin puanladı
    Finalized          = 2,   // Kesinleşmiş listede, User oluşturuldu
    DocumentsSubmitted = 3,   // Kesin kayıt evrakları yüklendi
    Approved           = 4,   // Admin onayladı — tam erişim
    Rejected           = 5    // Reddedildi
}
