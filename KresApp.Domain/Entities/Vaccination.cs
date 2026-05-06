using System;

namespace KresApp.Domain.Entities;

public class Vaccination
{
    public Guid Id { get; set; }
    public Guid ChildId { get; set; }
    public virtual Child Child { get; set; } = null!;
    public string VaccineName { get; set; } = null!;
    public DateTime? DateAdministered { get; set; }
    public DateTime? ScheduledDate { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, Completed, Overdue
    public string? Dose { get; set; } // e.g., "1. Doz", "Hepta-1"
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}
