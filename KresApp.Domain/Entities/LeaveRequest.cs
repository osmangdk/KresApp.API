using System;
using KresApp.Domain.Enums;

namespace KresApp.Domain.Entities;

public class LeaveRequest
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid ChildId { get; private set; }
    public Child Child { get; private set; } = null!;
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public string Reason { get; private set; } = string.Empty;
    public LeaveStatus Status { get; private set; } = LeaveStatus.Pending;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public Guid? ApprovedByUserId { get; private set; }
    public User? ApprovedByUser { get; private set; }
    public string? AdminNote { get; private set; }

    private LeaveRequest() { }

    public LeaveRequest(Guid childId, DateTime startDate, DateTime endDate, string reason)
    {
        ChildId = childId;
        StartDate = startDate.Date;
        EndDate = endDate.Date;
        Reason = reason;
    }

    public void Approve(Guid approvedByUserId, string? adminNote = null)
    {
        Status = LeaveStatus.Approved;
        ApprovedByUserId = approvedByUserId;
        AdminNote = adminNote;
    }

    public void Reject(Guid approvedByUserId, string? adminNote = null)
    {
        Status = LeaveStatus.Rejected;
        ApprovedByUserId = approvedByUserId;
        AdminNote = adminNote;
    }
}
