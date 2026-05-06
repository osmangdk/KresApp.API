using System;
using System.Collections.Generic;

namespace KresApp.Application.DTOs;

public class ChildHealthRecordDto
{
    public Guid Id { get; set; }
    public Guid ChildId { get; set; }
    public string DiseaseName { get; set; } = null!;
    public DateTime? OccurrenceDate { get; set; }
    public string? Description { get; set; }
    public string Category { get; set; } = null!;
}

public class CreateChildHealthRecordDto
{
    public Guid ChildId { get; set; }
    public string DiseaseName { get; set; } = null!;
    public DateTime? OccurrenceDate { get; set; }
    public string? Description { get; set; }
    public string Category { get; set; } = "Contagious";
}

public class UpdateChildHealthRecordDto
{
    public Guid Id { get; set; }
    public string DiseaseName { get; set; } = null!;
    public DateTime? OccurrenceDate { get; set; }
    public string? Description { get; set; }
    public string Category { get; set; } = null!;
}

public class ChildHealthHistoryDto
{
    public Guid ChildId { get; set; }
    public Guid ParentId { get; set; }
    public string ChildName { get; set; } = null!;
    public string? Gender { get; set; }
    public DateOnly? BirthDate { get; set; }
    public List<ChildHealthRecordDto> ContagiousDiseases { get; set; } = new();
    public List<ChildHealthRecordDto> ChronicDiseases { get; set; } = new();
    public string? MedicalNotes { get; set; }
}
