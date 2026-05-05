namespace KresApp.Application.DTOs;

using KresApp.Domain.Enums;

public class RegisterDto
{
    public string Email { get; set; }
    public string Password { get; set; }
    public UserRole Role { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
}

public class LoginDto
{
    public string Email { get; set; }
    public string Password { get; set; }
}