namespace KresApp.Application.DTOs;

public class LearningOutcomeDto
{
    public Guid Id { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public string MonthName { get; set; } = null!;
    public string Theme { get; set; } = null!;
    public List<string> Outcomes { get; set; } = new();
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateLearningOutcomeDto
{
    public int Month { get; set; }
    public int Year { get; set; }
    public string Theme { get; set; } = null!;
    public List<string> Outcomes { get; set; } = new();
    public string? Description { get; set; }
}
