namespace KresApp.Domain.Entities;

public class AgeGroup
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = null!; // "0-2 Yaş", "3-4 Yaş" vb.
    public int Quota { get; set; } // Kontenjan
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public List<Class> Classes { get; set; } = new();
    public List<LearningOutcome> LearningOutcomes { get; set; } = new();
}
