namespace KresApp.Domain.Entities;

public class LearningOutcome
{
    public Guid Id { get; set; }
    public int Month { get; set; }       // 1-12
    public int Year { get; set; }
    public string Theme { get; set; } = null!;
    public Guid? AgeGroupId { get; set; }
    public AgeGroup? AgeGroup { get; set; }
    public Guid? ClassId { get; set; }
    public Class? Class { get; set; }
    public List<string> Outcomes { get; set; } = new();
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}
