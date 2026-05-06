using KresApp.Domain.Enums;

namespace KresApp.Domain.Entities;

public class SchoolBellRequest
{
    public Guid Id { get; set; }
    public Guid ChildId { get; set; }
    public Guid ParentId { get; set; }
    public SchoolBellType Type { get; set; }
    public SchoolBellStatus Status { get; set; }
    public string? Note { get; set; }
    public DateTime EstimatedTime { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public Child Child { get; set; } = null!;
    public User Parent { get; set; } = null!;
}
