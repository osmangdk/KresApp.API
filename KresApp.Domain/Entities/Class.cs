namespace KresApp.Domain.Entities;

public class Class
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; } = null!;
    public Guid TeacherId { get; private set; }

    private Class() { }

    public Class(string name, Guid teacherId)
    {
        Name = name;
        TeacherId = teacherId;
    }

    public void Update(string name, Guid teacherId)
    {
        Name = name;
        TeacherId = teacherId;
    }
}