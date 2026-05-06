using System;

namespace KresApp.Domain.Entities;

public class ChildHealthRecord
{
    public Guid Id { get; set; }
    public Guid ChildId { get; set; }
    public virtual Child Child { get; set; } = null!;
    public string DiseaseName { get; set; } = null!;
    public DateTime? OccurrenceDate { get; set; }
    public string? Description { get; set; }
    public string Category { get; set; } = "Contagious"; // Contagious or Chronic
    public DateTime CreatedAt { get; set; }
}
