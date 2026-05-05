namespace KresApp.Application.DTOs;

public class MealMenuDto
{
    public Guid Id { get; set; }
    public string Day { get; set; } = null!;
    public string Breakfast { get; set; } = null!;
    public string Lunch { get; set; } = null!;
    public string Snack { get; set; } = null!;
}

public class CreateMealMenuDto
{
    public string Day { get; set; } = null!;
    public string Breakfast { get; set; } = null!;
    public string Lunch { get; set; } = null!;
    public string Snack { get; set; } = null!;
}
