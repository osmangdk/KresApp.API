namespace KresApp.Application.DTOs;

public class ScheduleDto
{
    public Guid Id { get; set; }
    public string Day { get; set; } = null!;
    public string Subject { get; set; } = null!;
    public string StartTime { get; set; } = null!;
    public string EndTime { get; set; } = null!;
    public string Teacher { get; set; } = null!;
}

public class CreateScheduleDto
{
    public string Day { get; set; } = null!;
    public string Subject { get; set; } = null!;
    public string StartTime { get; set; } = null!;
    public string EndTime { get; set; } = null!;
    public string Teacher { get; set; } = null!;
}
