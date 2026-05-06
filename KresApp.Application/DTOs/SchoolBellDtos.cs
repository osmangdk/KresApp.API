using KresApp.Domain.Enums;

namespace KresApp.Application.DTOs;

public class SchoolBellRequestDto
{
    public Guid Id { get; set; }
    public Guid ChildId { get; set; }
    public string ChildName { get; set; } = null!;
    public Guid ParentId { get; set; }
    public string ParentName { get; set; } = null!;
    public SchoolBellType Type { get; set; }
    public SchoolBellStatus Status { get; set; }
    public string? Note { get; set; }
    public DateTime EstimatedTime { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateSchoolBellRequestDto
{
    public Guid ChildId { get; set; }
    public SchoolBellType Type { get; set; }
    public string? Note { get; set; }
    public DateTime EstimatedTime { get; set; }
}

public class UpdateSchoolBellStatusDto
{
    public SchoolBellStatus Status { get; set; }
}
