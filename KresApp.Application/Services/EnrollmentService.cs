using KresApp.Application.DTOs;
using KresApp.Application.Interfaces;
using KresApp.Domain.Entities;
using KresApp.Domain.Enums;

namespace KresApp.Application.Services;

public class EnrollmentService
{
    private readonly IEnrollmentRequestRepository _repo;
    private readonly IUserRepository _users;
    private readonly IChildRepository _children;
    private readonly IPasswordHasher _hasher;
    private readonly IEmailService _email;
    private readonly IClassRepository _classes;

    public EnrollmentService(
        IEnrollmentRequestRepository repo,
        IUserRepository users,
        IChildRepository children,
        IPasswordHasher hasher,
        IEmailService email,
        IClassRepository classes)
    {
        _repo = repo;
        _users = users;
        _children = children;
        _hasher = hasher;
        _email = email;
        _classes = classes;
    }

    // ── 1. Ön Başvuru (anonim — basit bilgiler) ─────────────────────────────
    public async Task<Guid> SubmitAsync(CreateEnrollmentRequestDto dto)
    {
        var existingUser = await _users.GetByEmail(dto.ParentEmail);
        if (existingUser != null)
            throw new Exception("Bu e-posta adresi zaten sisteme kayıtlı.");

        var existingReq = await _repo.GetByEmailAsync(dto.ParentEmail);
        if (existingReq != null && existingReq.Status == EnrollmentStatus.Pending)
            throw new Exception("Bu e-posta için zaten bekleyen bir başvurunuz bulunuyor.");

        var request = new EnrollmentRequest
        {
            ParentName         = dto.ParentName,
            ParentEmail        = dto.ParentEmail,
            ParentPhone        = dto.ParentPhone,
            ParentTcKimlikNo   = dto.ParentTcKimlikNo,
            ParentPassword     = _hasher.Hash(dto.ParentPassword),
            ParentJob          = dto.ParentJob,
            ParentSicilNo      = dto.ParentSicilNo,
            ParentUnit         = dto.ParentUnit,
            ParentTitle        = dto.ParentTitle,
            ParentServiceYears = dto.ParentServiceYears,
            ParentWorkAddress  = dto.ParentWorkAddress,
            ParentHomeAddress  = dto.ParentHomeAddress,

            FatherName         = dto.FatherName,
            FatherPhone        = dto.FatherPhone,
            FatherTcKimlikNo   = dto.FatherTcKimlikNo,
            FatherJob          = dto.FatherJob,
            FatherWorkAddress  = dto.FatherWorkAddress,

            SpouseIsAlive      = dto.SpouseIsAlive,
            SpouseIsWorking    = dto.SpouseIsWorking,
            SpouseWorkplace    = dto.SpouseWorkplace,
            SpouseWorkplaceHasDaycare = dto.SpouseWorkplaceHasDaycare,

            ChildFullName      = dto.ChildFullName,
            ChildTcKimlikNo    = dto.ChildTcKimlikNo,
            ChildBirthDate     = dto.ChildBirthDate,
            ChildGender        = dto.ChildGender,
            ChildBloodType     = dto.ChildBloodType,
            ChildAllergies     = dto.ChildAllergies,
            Notes              = dto.Notes,
        };

        await _repo.AddAsync(request);

        // Send pre-registration confirmation email
        var emailBody = $@"
<div style='font-family: Arial, sans-serif; padding: 20px; color: #333; max-width: 600px; margin: auto; border: 1px solid #eee; border-radius: 8px;'>
    <h2 style='color: #0066cc; border-bottom: 2px solid #0066cc; padding-bottom: 10px;'>ASHB Kreş Ön Başvurusu Alındı</h2>
    <p>Sayın <strong>{dto.ParentName}</strong>,</p>
    <p>Kreş Yönetim Sistemi'ne yapmış olduğunuz ön başvuru başarıyla sistemimize kaydedilmiştir.</p>
    <p>Başvurunuz yetkililer tarafından en kısa sürede değerlendirilerek puanlanacaktır. Gelişmeler hakkında sizi e-posta ve SMS yoluyla bilgilendireceğiz.</p>
    <div style='background-color: #f9f9f9; padding: 15px; border-radius: 5px; margin-top: 20px;'>
        <strong>Başvuru Bilgileri:</strong><br/>
        • Veli Adı: {dto.ParentName}<br/>
        • Çocuk Adı: {dto.ChildFullName}<br/>
        • Başvuru Tarihi: {DateTime.Now:dd.MM.yyyy HH:mm}
    </div>
    <p style='margin-top: 20px; font-size: 12px; color: #777;'>Bu e-posta otomatik olarak gönderilmiştir. Lütfen yanıtlamayınız.</p>
</div>";

        await _email.SendEmailAsync(dto.ParentEmail, "ASHB Kreş Ön Başvurunuz Alındı", emailBody);

        return request.Id;
    }

    // ── 2. Admin: Puanlama ──────────────────────────────────────────────────
    public async Task ScoreAsync(Guid id, int score, string? notes, Guid reviewerUserId)
    {
        var req = await _repo.GetByIdAsync(id)
            ?? throw new Exception("Başvuru bulunamadı.");

        if (req.Status != EnrollmentStatus.Pending && req.Status != EnrollmentStatus.Scored)
            throw new Exception("Bu başvuru puanlanamaz (zaten işleme alınmış).");

        req.Score            = score;
        req.ScoringNotes     = notes;
        req.Status           = EnrollmentStatus.Scored;
        req.ReviewedByUserId = reviewerUserId;
        await _repo.UpdateAsync(req);
    }

    // ── 3. Admin: Kesinleştir (User hesabı oluştur) ─────────────────────────
    public async Task FinalizeAsync(Guid id, Guid reviewerUserId)
    {
        var req = await _repo.GetByIdAsync(id)
            ?? throw new Exception("Başvuru bulunamadı.");

        if (req.Status != EnrollmentStatus.Scored && req.Status != EnrollmentStatus.Pending)
            throw new Exception("Bu başvuru kesinleştirilemez.");

        // Veli hesabı oluştur — PendingDocuments statüsüyle
        var parent = new User(
            req.ParentEmail, req.ParentPassword, UserRole.Parent,
            req.ParentName, req.ParentPhone, req.ParentTcKimlikNo,
            AccountStatus.PendingDocuments);
        await _users.AddAsync(parent);

        req.Status           = EnrollmentStatus.Finalized;
        req.CreatedUserId    = parent.Id;
        req.ReviewedAt       = DateTime.UtcNow;
        req.ReviewedByUserId = reviewerUserId;
        await _repo.UpdateAsync(req);

        // Send approval and document upload request email
        var emailBody = $@"
<div style='font-family: Arial, sans-serif; padding: 20px; color: #333; max-width: 600px; margin: auto; border: 1px solid #eee; border-radius: 8px;'>
    <h2 style='color: #28a745; border-bottom: 2px solid #28a745; padding-bottom: 10px;'>ASHB Kreş Ön Başvurunuz Onaylandı!</h2>
    <p>Sayın <strong>{req.ParentName}</strong>,</p>
    <p>Kreş ön başvurunuz yetkililer tarafından incelenmiş ve <strong>onaylanmıştır</strong>.</p>
    <p>Kesin kayıt işlemlerine devam edebilmek için sisteme giriş yaparak gerekli evrakları (Çocuk fotoğrafı, nüfus cüzdanı sureti, sağlık raporu vb.) yüklemeniz gerekmektedir.</p>
    <div style='background-color: #f9f9f9; padding: 15px; border-radius: 5px; margin-top: 20px;'>
        <strong>Giriş Bilgileri:</strong><br/>
        • Giriş Adresi: ASHB Kreş Portalı<br/>
        • Kullanıcı Adı: {req.ParentEmail}<br/>
        • Şifre: Başvuru esnasında belirlemiş olduğunuz şifreniz
    </div>
    <p>Lütfen en kısa sürede sisteme giriş yapıp evraklarınızı yükleyiniz.</p>
    <p style='margin-top: 20px; font-size: 12px; color: #777;'>Bu e-posta otomatik olarak gönderilmiştir. Lütfen yanıtlamayınız.</p>
</div>";

        await _email.SendEmailAsync(req.ParentEmail, "ASHB Kreş Ön Başvurunuz Onaylandı - Evrak Yükleme Talebi", emailBody);
    }

    // ── 4. Veli: Evrakları tamamla ──────────────────────────────────────────
    public async Task SubmitDocumentsAsync(Guid userId)
    {
        var user = await _users.GetByIdAsync(userId)
            ?? throw new Exception("Kullanıcı bulunamadı.");

        if (user.AccountStatus != AccountStatus.PendingDocuments)
            throw new Exception("Evrak yükleme aşamasında değilsiniz.");

        // EnrollmentRequest'i bul
        var allReqs = await _repo.GetAllAsync();
        var req = allReqs.FirstOrDefault(r => r.CreatedUserId == userId)
            ?? throw new Exception("Başvuru kaydı bulunamadı.");

        req.Status = EnrollmentStatus.DocumentsSubmitted;
        await _repo.UpdateAsync(req);

        user.SetAccountStatus(AccountStatus.DocumentsSubmitted);
        await _users.UpdateAsync(user);
    }

    // ── 5. Admin: Evrakları onayla → tam erişim ─────────────────────────────
    public async Task ApproveDocumentsAsync(Guid enrollmentId, ApproveEnrollmentDto dto, Guid reviewerUserId)
    {
        var req = await _repo.GetByIdAsync(enrollmentId)
            ?? throw new Exception("Başvuru bulunamadı.");

        if (req.Status != EnrollmentStatus.DocumentsSubmitted)
            throw new Exception("Bu başvurunun evrakları henüz tamamlanmamış.");

        var user = req.CreatedUserId.HasValue
            ? await _users.GetByIdAsync(req.CreatedUserId.Value)
            : null;
        if (user == null) throw new Exception("İlişkili kullanıcı bulunamadı.");

        // Çocuk kaydı oluştur
        var child = new Child(req.ChildFullName, user.Id, dto.ClassId, req.ChildBirthDate, req.ChildTcKimlikNo);
        child.UpdateProfile(
            req.ChildFullName, req.ChildBirthDate,
            req.ChildBloodType ?? "Tamamlanmadı",
            dto.ClassId,
            req.ParentName, req.ParentPhone,
            null, req.ChildGender,
            null, null, req.ChildTcKimlikNo);

        if (!string.IsNullOrWhiteSpace(req.OtherHealthNotes))
            child.UpdateMedicalNotes(req.OtherHealthNotes);

        await _children.AddAsync(child);

        // Durumları güncelle
        req.Status           = EnrollmentStatus.Approved;
        req.AdminNote        = dto.AdminNote;
        req.ReviewedAt       = DateTime.UtcNow;
        req.ReviewedByUserId = reviewerUserId;
        await _repo.UpdateAsync(req);

        user.SetAccountStatus(AccountStatus.Active);
        await _users.UpdateAsync(user);

        // Fetch class info for email
        var classEntity = await _classes.GetByIdAsync(dto.ClassId);
        var className = classEntity?.Name ?? "Belirtilmedi";

        // Send final enrollment success email
        var emailBody = $@"
<div style='font-family: Arial, sans-serif; padding: 20px; color: #333; max-width: 600px; margin: auto; border: 1px solid #eee; border-radius: 8px;'>
    <h2 style='color: #0066cc; border-bottom: 2px solid #0066cc; padding-bottom: 10px;'>Kesin Kaydınız Tamamlandı! 🎉</h2>
    <p>Sayın <strong>{req.ParentName}</strong>,</p>
    <p>Gerekli evraklarınız yetkililerce incelenmiş, onaylanmış ve <strong>{req.ChildFullName}</strong> isimli çocuğunuzun kesin kaydı başarıyla tamamlanmıştır.</p>
    <div style='background-color: #f9f9f9; padding: 15px; border-radius: 5px; margin-top: 20px;'>
        <strong>Kayıt ve Sınıf Bilgileri:</strong><br/>
        • Öğrenci Adı: {req.ChildFullName}<br/>
        • Atandığı Sınıf: {className}<br/>
        • Hesap Durumu: Aktif
    </div>
    <p>Artık ASHB Kreş Yönetim Sistemi mobil/web uygulamasına giriş yaparak çocuğunuzun günlük durumunu, ders programını, yemek menüsünü ve duyuruları takip edebilirsiniz.</p>
    <p style='margin-top: 20px; font-size: 12px; color: #777;'>Bu e-posta otomatik olarak gönderilmiştir. Lütfen yanıtlamayınız.</p>
</div>";

        await _email.SendEmailAsync(req.ParentEmail, "ASHB Kreş Kesin Kaydınız Tamamlandı 🎉", emailBody);
    }

    // ── 6. Admin: Reddet ────────────────────────────────────────────────────
    public async Task RejectAsync(Guid id, RejectEnrollmentDto dto, Guid reviewerUserId)
    {
        var req = await _repo.GetByIdAsync(id)
            ?? throw new Exception("Başvuru bulunamadı.");

        req.Status           = EnrollmentStatus.Rejected;
        req.AdminNote        = dto.AdminNote;
        req.ReviewedAt       = DateTime.UtcNow;
        req.ReviewedByUserId = reviewerUserId;
        await _repo.UpdateAsync(req);

        // Send rejection email
        var emailBody = $@"
<div style='font-family: Arial, sans-serif; padding: 20px; color: #333; max-width: 600px; margin: auto; border: 1px solid #eee; border-radius: 8px;'>
    <h2 style='color: #dc3545; border-bottom: 2px solid #dc3545; padding-bottom: 10px;'>Başvuru Durumu Bilgilendirmesi</h2>
    <p>Sayın <strong>{req.ParentName}</strong>,</p>
    <p>Kreş Yönetim Sistemi'ne yapmış olduğunuz başvurunuz yetkililer tarafından değerlendirilmiş olup, maalesef <strong>olumlu sonuçlandırılamamıştır</strong>.</p>
    <div style='background-color: #fff3f3; border: 1px solid #f5c6cb; padding: 15px; border-radius: 5px; margin-top: 20px; color: #721c24;'>
        <strong>Gerekçe / Açıklama:</strong><br/>
        {dto.AdminNote}
    </div>
    <p>Başvurunuzla ilgilendiğiniz için teşekkür ederiz.</p>
    <p style='margin-top: 20px; font-size: 12px; color: #777;'>Bu e-posta otomatik olarak gönderilmiştir. Lütfen yanıtlamayınız.</p>
</div>";

        await _email.SendEmailAsync(req.ParentEmail, "ASHB Kreş Başvurunuz Hakkında Bilgilendirme", emailBody);
    }

    // ── Listeler ─────────────────────────────────────────────────────────────
    public async Task<List<EnrollmentRequestDto>> GetAllAsync()
        => (await _repo.GetAllAsync()).Select(ToDto).ToList();

    public async Task<List<EnrollmentRequestDto>> GetPendingAsync()
        => (await _repo.GetAllAsync()).Where(r => r.Status == EnrollmentStatus.Pending || r.Status == EnrollmentStatus.Scored)
            .OrderByDescending(r => r.Score ?? 0).Select(ToDto).ToList();

    public async Task<List<EnrollmentRequestDto>> GetFinalizedAsync()
        => (await _repo.GetAllAsync()).Where(r => r.Status == EnrollmentStatus.Finalized || r.Status == EnrollmentStatus.DocumentsSubmitted)
            .Select(ToDto).ToList();

    // ── Mapper ───────────────────────────────────────────────────────────────
    private static EnrollmentRequestDto ToDto(EnrollmentRequest r) => new(
        r.Id, r.ParentName, r.ParentEmail, r.ParentPhone, r.ParentTcKimlikNo,
        r.ChildFullName, r.ChildTcKimlikNo, r.ChildBirthDate, r.ChildGender,
        r.ChildBloodType, r.ChildAllergies,
        r.Score, r.ScoringNotes,
        r.Notes, r.Status.ToString(), r.AdminNote,
        r.CreatedAt, r.ReviewedAt,
        r.ParentSicilNo, r.ParentUnit, r.ParentTitle, r.ParentServiceYears,
        r.SpouseIsAlive, r.SpouseIsWorking, r.SpouseWorkplace, r.SpouseWorkplaceHasDaycare
    );
}
