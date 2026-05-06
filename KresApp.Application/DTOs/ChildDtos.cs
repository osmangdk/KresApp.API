namespace KresApp.Application.DTOs;

public class UpdateChildDto
{
    public string Name { get; set; } = null!;
    public DateOnly? BirthDate { get; set; }
    public string BloodType { get; set; } = null!;
    public Guid ClassId { get; set; }
    public string ParentName { get; set; } = null!;
    public string ParentPhone { get; set; } = null!;
    public string? SecondaryPhone { get; set; }
    public string? Gender { get; set; }
    public double? Weight { get; set; }
    public double? Height { get; set; }
}

// Tek bir alerji kaydı eklemek/güncellemek için
public class ChildAllergyDto
{
    public Guid? Id { get; set; }
    public string AllergyName { get; set; } = null!;
    public string? Severity { get; set; }   // Hafif, Orta, Ağır
    public string? Notes { get; set; }
}

// Toplu alerji güncelleme + medikal notlar
public class UpdateAllergiesDto
{
    public List<ChildAllergyDto> Allergies { get; set; } = new();
    public string? MedicalNotes { get; set; }
}

// Alerji bilgisi response DTO
public class ChildAllergyResponseDto
{
    public Guid Id { get; set; }
    public string AllergyName { get; set; } = null!;
    public string? Severity { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ChildDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public Guid ParentId { get; set; }
    public DateOnly? BirthDate { get; set; }
    public Guid ClassId { get; set; }
    public string ClassName { get; set; } = null!;
    public string BloodType { get; set; } = null!;
    public List<ChildAllergyResponseDto> Allergies { get; set; } = new();
    public string? MedicalNotes { get; set; }
    public string ParentName { get; set; } = null!;
    public string ParentPhone { get; set; } = null!;
    public string? SecondaryPhone { get; set; }
    public string? Gender { get; set; }
    public double? Weight { get; set; }
    public double? Height { get; set; }
    public DateOnly EnrollmentDate { get; set; }
}
