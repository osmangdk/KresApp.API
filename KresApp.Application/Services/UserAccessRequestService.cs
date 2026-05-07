using KresApp.Application.DTOs;
using KresApp.Application.Interfaces;
using KresApp.Domain.Entities;

namespace KresApp.Application.Services;

public class UserAccessRequestService
{
    private readonly IUserAccessRequestRepository _repo;
    private readonly IUserRepository _users;
    private readonly ILdapService _ldap;

    public UserAccessRequestService(
        IUserAccessRequestRepository repo,
        IUserRepository users,
        ILdapService ldap)
    {
        _repo = repo;
        _users = users;
        _ldap = ldap;
    }

    /// <summary>
    /// LDAP doğrulaması başarılı olan kullanıcı erişim talebi oluşturur.
    /// </summary>
    public async Task CreateRequestAsync(string email, CreateAccessRequestDto dto)
    {
        // Zaten kayıtlı kullanıcı mı?
        var existing = await _users.GetByEmail(email);
        if (existing != null)
            throw new Exception("Bu e-posta adresi zaten sisteme kayıtlı.");

        // Daha önce bekleyen talep var mı?
        var existingRequest = await _repo.GetByEmailAsync(email);
        if (existingRequest != null && existingRequest.Status == AccessRequestStatus.Pending)
            throw new Exception("Bu e-posta için zaten bekleyen bir erişim talebiniz bulunuyor.");

        var request = new UserAccessRequest
        {
            Email = email,
            Name = dto.Name,
            Phone = dto.Phone,
            Status = AccessRequestStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(request);
    }

    /// <summary>
    /// Bekleyen tüm talepleri listeler (Admin/SuperAdmin).
    /// </summary>
    public async Task<List<AccessRequestDto>> GetPendingAsync()
    {
        var requests = await _repo.GetAllPendingAsync();
        return requests.Select(r => new AccessRequestDto(
            r.Id, r.Email, r.Name, r.Phone,
            r.Status.ToString(), r.CreatedAt
        )).ToList();
    }

    /// <summary>
    /// Admin talebi onaylar, rol atar ve kullanıcıyı oluşturur.
    /// </summary>
    public async Task ApproveAsync(Guid id, ApproveAccessRequestDto dto)
    {
        var request = await _repo.GetByIdAsync(id)
            ?? throw new Exception("Talep bulunamadı.");

        if (request.Status != AccessRequestStatus.Pending)
            throw new Exception("Bu talep zaten işleme alınmış.");

        // LDAP kullanıcısı olarak oluştur (şifre DB'de tutulmaz)
        var user = new User(request.Email, "LDAP_USER", dto.Role, request.Name, request.Phone);
        await _users.AddAsync(user);

        request.Status = AccessRequestStatus.Approved;
        request.AdminNote = dto.AdminNote;
        request.ProcessedAt = DateTime.UtcNow;
        await _repo.UpdateAsync(request);
    }

    /// <summary>
    /// Admin talebi reddeder.
    /// </summary>
    public async Task RejectAsync(Guid id, string? adminNote)
    {
        var request = await _repo.GetByIdAsync(id)
            ?? throw new Exception("Talep bulunamadı.");

        if (request.Status != AccessRequestStatus.Pending)
            throw new Exception("Bu talep zaten işleme alınmış.");

        request.Status = AccessRequestStatus.Rejected;
        request.AdminNote = adminNote;
        request.ProcessedAt = DateTime.UtcNow;
        await _repo.UpdateAsync(request);
    }
}
