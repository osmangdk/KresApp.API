namespace KresApp.Domain.Entities;

public class Schedule
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Day { get; private set; } = null!; // Pazartesi vs.
    public string Subject { get; private set; } = null!;
    public string StartTime { get; private set; } = null!; // 09:00 vs.
    public string EndTime { get; private set; } = null!;
    public string Teacher { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }

    private Schedule() { }

    public Schedule(string day, string subject, string startTime, string endTime, string teacher)
    {
        Day = day;
        Subject = subject;
        StartTime = startTime;
        EndTime = endTime;
        Teacher = teacher;
    }

    public void Update(string day, string subject, string startTime, string endTime, string teacher)
    {
        Day = day;
        Subject = subject;
        StartTime = startTime;
        EndTime = endTime;
        Teacher = teacher;
    }
}
