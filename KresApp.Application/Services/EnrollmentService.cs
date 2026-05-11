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

    public EnrollmentService(
        IEnrollmentRequestRepository repo,
        IUserRepository users,
        IChildRepository children,
        IPasswordHasher hasher)
    {
        _repo = repo;
        _users = users;
        _children = children;
        _hasher = hasher;
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
