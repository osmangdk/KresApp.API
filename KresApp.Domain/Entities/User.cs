using KresApp.Domain.Enums;

namespace KresApp.Domain.Entities;

public class User
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public UserRole Role { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Phone { get; private set; }

    // EF için boş ctor
    public User()
    {
        
    }

    public User(string email, string passwordHash, UserRole role, string name = "", string? phone = null)
    {
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
        Name = name;
        Phone = phone;
    }

    public void ChangePassword(string newHash)
    {
        PasswordHash = newHash;
    }

    public void UpdateProfile(string name, string? phone)
    {
        Name = name;
        Phone = phone;
    }

    public void UpdateEmail(string email)
    {
        Email = email;
    }

    public void UpdateRole(UserRole role)
    {
        Role = role;
    }
}