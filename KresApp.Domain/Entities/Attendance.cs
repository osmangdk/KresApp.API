using KresApp.Domain.Enums;

namespace KresApp.Domain.Entities;

public class Attendance
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid ChildId { get; private set; }
    public DateOnly Date { get; private set; }
    public AttendanceStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Attendance() { }

    public Attendance(Guid childId, DateOnly date, AttendanceStatus status)
    {
        ChildId = childId;
        Date = date;
        Status = status;
    }

    public void UpdateStatus(AttendanceStatus newStatus)
    {
        Status = newStatus;
    }
}
