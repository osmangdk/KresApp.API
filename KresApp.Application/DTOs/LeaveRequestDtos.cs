using KresApp.Domain.Enums;

namespace KresApp.Application.DTOs;

public class LeaveRequestDto
{
    public Guid Id { get; set; }
    public Guid ChildId { get; set; }
    public string ChildFullName { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Reason { get; set; } = null!;
    public LeaveStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? AdminNote { get; set; }
}

public class CreateLeaveRequestDto
{
    public Guid ChildId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Reason { get; set; } = null!;
}

public class ApproveLeaveRequestDto
{
    public string? AdminNote { get; set; }
}
