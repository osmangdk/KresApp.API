namespace KresApp.Domain.Entities;

public class MealMenu
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Day { get; private set; } = null!; // Pazartesi, Salı vs.
    public DateTime Date { get; private set; } // Tam tarih
    public string Breakfast { get; private set; } = null!;
    public string Lunch { get; private set; } = null!;
    public string Snack { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }

    private MealMenu() { }

    public MealMenu(string day, DateTime date, string breakfast, string lunch, string snack)
    {
        Day = day;
        Date = date;
        Breakfast = breakfast;
        Lunch = lunch;
        Snack = snack;
        CreatedAt = DateTime.UtcNow;
    }

    public void Update(string day, DateTime date, string breakfast, string lunch, string snack)
    {
        Day = day;
        Date = date;
        Breakfast = breakfast;
        Lunch = lunch;
        Snack = snack;
    }
}
