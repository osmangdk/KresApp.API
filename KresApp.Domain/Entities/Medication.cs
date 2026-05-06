using System;
using System.Collections.Generic;

namespace KresApp.Domain.Entities;

public class Medication
{
    public Guid Id { get; set; }
    public Guid ChildId { get; set; }
    public virtual Child Child { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Dosage { get; set; } = null!;
    public List<string> Times { get; set; } = new(); // e.g., ["09:30", "15:30"]
    public string? Note { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
}

public class MedicationLog
{
    public Guid Id { get; set; }
    public Guid MedicationId { get; set; }
    public virtual Medication Medication { get; set; } = null!;
    public DateTime Date { get; set; }
    public string Time { get; set; } = null!;
    public bool IsGiven { get; set; }
    public Guid? GivenByUserId { get; set; }
    public virtual User? GivenBy { get; set; }
    public DateTime? GivenAt { get; set; }
}
