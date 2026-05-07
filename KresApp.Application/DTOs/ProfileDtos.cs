namespace KresApp.Application.DTOs;

public class ProfileDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Phone { get; set; }
    public string Role { get; set; } = null!;
    public List<Guid>? ChildIds { get; set; }
}

public class UpdateProfileDto
{
    public string Name { get; set; } = null!;
    public string? Phone { get; set; }
}

public class ChangePasswordDto
{
    public string CurrentPassword { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
}
