using KresApp.Domain.Enums;

namespace KresApp.Domain.Entities;

public class UserAccessRequest
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Email { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? Phone { get; set; }
    public UserRole RequestedRole { get; set; }
    public AccessRequestStatus Status { get; set; } = AccessRequestStatus.Pending;
    public string? AdminNote { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ProcessedAt { get; set; }
}

public enum AccessRequestStatus
{
    Pending = 0,
    Approved = 1,
    Rejected = 2
}
