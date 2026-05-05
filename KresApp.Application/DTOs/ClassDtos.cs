namespace KresApp.Application.DTOs;

public class ClassDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public Guid TeacherId { get; set; }
}

public class CreateClassDto
{
    public string Name { get; set; } = null!;
    public Guid TeacherId { get; set; }
}
