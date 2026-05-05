namespace KresApp.Domain.Entities;

public class ChildAllergy
{
    public Guid Id { get; private set; }
    public Guid ChildId { get; private set; }
    public string AllergyName { get; private set; } = null!;
    public string? Severity { get; private set; }   // Hafif, Orta, Ağır
    public string? Notes { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Navigation
    public Child Child { get; private set; } = null!;

    private ChildAllergy() { }

    public ChildAllergy(Guid childId, string allergyName, string? severity = null, string? notes = null)
    {
        ChildId = childId;
        AllergyName = allergyName;
        Severity = severity;
        Notes = notes;
    }

    public void Update(string allergyName, string? severity, string? notes)
    {
        AllergyName = allergyName;
        Severity = severity;
        Notes = notes;
    }
}
