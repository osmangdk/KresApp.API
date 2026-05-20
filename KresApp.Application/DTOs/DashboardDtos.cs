namespace KresApp.Application.DTOs;

public class DashboardStatsDto
{
    public int TotalStudents { get; set; }
    public int PresentToday { get; set; }
    public double AttendanceRate { get; set; }
    public int PendingPayments { get; set; }
    public int OverduePayments { get; set; }
    public int TotalStaff { get; set; }
    public int TotalTeachers { get; set; }
    public int TotalParents { get; set; }
    public int TotalClasses { get; set; }
    public int PendingEnrollments { get; set; }
    public int PendingAccessRequests { get; set; }
    public WeeklyAttendanceDto WeeklyAttendance { get; set; } = new();
}

public class DashboardNotificationDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Body { get; set; } = null!;
    public string TypeIcon { get; set; } = null!;
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class WeeklyAttendanceDto
{
    public List<double> Rates { get; set; } = new();
}
