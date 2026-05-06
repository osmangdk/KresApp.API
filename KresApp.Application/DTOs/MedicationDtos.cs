using System;
using System.Collections.Generic;

namespace KresApp.Application.DTOs;

public class MedicationDto
{
    public Guid Id { get; set; }
    public Guid ChildId { get; set; }
    public string Name { get; set; } = null!;
    public string Dosage { get; set; } = null!;
    public List<string> Times { get; set; } = new();
    public string? Note { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; }
    public List<MedicationLogDto> TodayLogs { get; set; } = new();
}

public class MedicationLogDto
{
    public Guid? Id { get; set; }
    public string Time { get; set; } = null!;
    public bool IsGiven { get; set; }
    public Guid? GivenByUserId { get; set; }
    public string? GivenByName { get; set; }
    public DateTime? GivenAt { get; set; }
}

public class CreateMedicationDto
{
    public Guid ChildId { get; set; }
    public string Name { get; set; } = null!;
    public string Dosage { get; set; } = null!;
    public List<string> Times { get; set; } = new();
    public string? Note { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

public class GiveMedicationDto
{
    public Guid MedicationId { get; set; }
    public DateTime Date { get; set; }
    public string Time { get; set; } = null!;
}
