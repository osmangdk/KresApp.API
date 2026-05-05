using KresApp.Domain.Enums;

namespace KresApp.Application.DTOs;

public class DailyReportDto
{
    public Guid Id { get; set; }
    public Guid ChildId { get; set; }
    public string ChildName { get; set; } = null!;
    public DateOnly Date { get; set; }
    public MoodStatus Mood { get; set; }
    public MealStatus MorningMeal { get; set; }
    public MealStatus LunchMeal { get; set; }
    public MealStatus AfternoonMeal { get; set; }
    public bool DidSleep { get; set; }
    public int SleepHours { get; set; }
    public int SleepMins { get; set; }
    public int ToiletCount { get; set; }
    public string[] Activities { get; set; } = Array.Empty<string>();
    public string? TeacherNote { get; set; }
}

public class CreateDailyReportDto
{
    public Guid ChildId { get; set; }
    public DateOnly Date { get; set; }
    public MoodStatus Mood { get; set; }
    public MealStatus MorningMeal { get; set; }
    public MealStatus LunchMeal { get; set; }
    public MealStatus AfternoonMeal { get; set; }
    public bool DidSleep { get; set; }
    public int SleepHours { get; set; }
    public int SleepMins { get; set; }
    public int ToiletCount { get; set; }
    public string[] Activities { get; set; } = Array.Empty<string>();
    public string? TeacherNote { get; set; }
}
