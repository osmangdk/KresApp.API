namespace KresApp.Application.DTOs;

public class AgeGroupDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public int Quota { get; set; }
    public string? Description { get; set; }
}

public class CreateAgeGroupDto
{
    public string Name { get; set; } = null!;
    public int Quota { get; set; }
    public string? Description { get; set; }
}
