using KresApp.Domain.Enums;

namespace KresApp.Domain.Entities;

public class DailyReport
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid ChildId { get; private set; }
    public DateOnly Date { get; private set; }
    public MoodStatus Mood { get; private set; }
    public MealStatus MorningMeal { get; private set; }
    public MealStatus LunchMeal { get; private set; }
    public MealStatus AfternoonMeal { get; private set; }
    public bool DidSleep { get; private set; }
    public int SleepHours { get; private set; }
    public int SleepMins { get; private set; }
    public int ToiletCount { get; private set; }
    public string[] Activities { get; private set; } = Array.Empty<string>();
    public string? TeacherNote { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private DailyReport() { }

    public DailyReport(Guid childId, DateOnly date, MoodStatus mood, 
        MealStatus morningMeal, MealStatus lunchMeal, MealStatus afternoonMeal, 
        bool didSleep, int sleepHours, int sleepMins, int toiletCount, 
        string[] activities, string? teacherNote)
    {
        ChildId = childId;
        Date = date;
        Mood = mood;
        MorningMeal = morningMeal;
        LunchMeal = lunchMeal;
        AfternoonMeal = afternoonMeal;
        DidSleep = didSleep;
        SleepHours = sleepHours;
        SleepMins = sleepMins;
        ToiletCount = toiletCount;
        Activities = activities;
        TeacherNote = teacherNote;
    }

    public void Update(MoodStatus mood, MealStatus morningMeal, MealStatus lunchMeal, MealStatus afternoonMeal, 
        bool didSleep, int sleepHours, int sleepMins, int toiletCount, string[] activities, string? teacherNote)
    {
        Mood = mood;
        MorningMeal = morningMeal;
        LunchMeal = lunchMeal;
        AfternoonMeal = afternoonMeal;
        DidSleep = didSleep;
        SleepHours = sleepHours;
        SleepMins = sleepMins;
        ToiletCount = toiletCount;
        Activities = activities;
        TeacherNote = teacherNote;
    }
}
