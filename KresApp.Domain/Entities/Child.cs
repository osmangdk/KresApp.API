using System.Collections.ObjectModel;

namespace KresApp.Domain.Entities;

public class Child
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public DateOnly? BirthDate { get; private set; }
    public Guid ParentId { get; private set; }
    public Guid ClassId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string BloodType { get; private set; } = "Tamamlanmadı";
    public string? MedicalNotes { get; private set; }

    // Navigation - 1:N ilişki
    public ICollection<ChildAllergy> Allergies { get; private set; } = new List<ChildAllergy>();
    public string ParentName { get; private set; } = string.Empty;
    public string ParentPhone { get; private set; } = string.Empty;
    public string? SecondaryPhone { get; private set; }
    public DateOnly EnrollmentDate { get; private set; }
    private Child()
    {
    }

    public Child(string name, Guid parentId, Guid classId, DateOnly? birthDate = null)
    {
        Name = name;
        ParentId = parentId;
        ClassId = classId;
        BirthDate = birthDate;
        EnrollmentDate = DateOnly.FromDateTime(DateTime.UtcNow);
    }

    public void UpdateMedicalNotes(string? medicalNotes)
    {
        MedicalNotes = medicalNotes;
    }

    public void UpdateProfile(string name, DateOnly? birthDate, string bloodType, Guid classId, string parentName, string parentPhone, string? secondaryPhone)
    {
        Name = name;
        BirthDate = birthDate;
        BloodType = bloodType;
        ClassId = classId;
        ParentName = parentName;
        ParentPhone = parentPhone;
        SecondaryPhone = secondaryPhone;
    }
}
