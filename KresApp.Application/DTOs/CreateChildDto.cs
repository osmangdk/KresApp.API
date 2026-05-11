using System;

namespace KresApp.Application.DTOs;

public class CreateChildDto
{
    public string Name { get; set; } = null!;
    public string? TcKimlikNo { get; set; }
    public DateOnly? BirthDate { get; set; }
    public Guid ClassId { get; set; }
    public Guid ParentId { get; set; }
    public string BloodType { get; set; } = "Tamamlanmadı";
    public string ParentName { get; set; } = string.Empty;
    public string ParentPhone { get; set; } = string.Empty;
    public string? SecondaryPhone { get; set; }
}
