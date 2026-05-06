using System;

namespace KresApp.Application.DTOs;

public class VaccinationDto
{
    public Guid Id { get; set; }
    public Guid ChildId { get; set; }
    public string VaccineName { get; set; } = null!;
    public DateTime? DateAdministered { get; set; }
    public DateTime? ScheduledDate { get; set; }
    public string Status { get; set; } = null!;
    public string? Dose { get; set; }
    public string? Notes { get; set; }
}

public class CreateVaccinationDto
{
    public Guid ChildId { get; set; }
    public string VaccineName { get; set; } = null!;
    public DateTime? DateAdministered { get; set; }
    public DateTime? ScheduledDate { get; set; }
    public string Status { get; set; } = "Pending";
    public string? Dose { get; set; }
    public string? Notes { get; set; }
}

public class UpdateVaccinationDto
{
    public Guid Id { get; set; }
    public string VaccineName { get; set; } = null!;
    public DateTime? DateAdministered { get; set; }
    public DateTime? ScheduledDate { get; set; }
    public string Status { get; set; } = null!;
    public string? Dose { get; set; }
    public string? Notes { get; set; }
}
