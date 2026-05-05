using KresApp.Domain.Enums;

namespace KresApp.Application.DTOs;

public class AttendanceDto
{
    public Guid Id { get; set; }
    public Guid ChildId { get; set; }
    public string ChildName { get; set; } = null!;
    public DateOnly Date { get; set; }
    public AttendanceStatus Status { get; set; }
}

public class CreateAttendanceRecordDto
{
    public Guid ChildId { get; set; }
    public AttendanceStatus Status { get; set; }
}

public class CreateBulkAttendanceDto
{
    public DateOnly Date { get; set; }
    public List<CreateAttendanceRecordDto> Records { get; set; } = new();
}

public class AttendanceSummaryDto
{
    public DateOnly Date { get; set; }
    public int Present { get; set; }
    public int Absent { get; set; }
    public int Late { get; set; }
    public int Total { get; set; }
    public double Rate { get; set; }
}
