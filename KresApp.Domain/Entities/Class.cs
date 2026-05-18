namespace KresApp.Domain.Entities;

public class Class
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; } = null!;
    public Guid TeacherId { get; private set; }
    public Guid? AgeGroupId { get; private set; }
    public AgeGroup? AgeGroup { get; private set; }

    private Class() { }

    public Class(string name, Guid teacherId, Guid? ageGroupId = null)
    {
        Name = name;
        TeacherId = teacherId;
        AgeGroupId = ageGroupId;
    }

    public void Update(string name, Guid teacherId, Guid? ageGroupId = null)
    {
        Name = name;
        TeacherId = teacherId;
        AgeGroupId = ageGroupId;
    }
}