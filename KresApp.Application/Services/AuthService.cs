using KresApp.Application.Interfaces;
using KresApp.Domain.Entities;
using KresApp.Domain.Enums;

namespace KresApp.Application.Services;

public class AuthService
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtService _jwt;

    public AuthService(IUserRepository users, IPasswordHasher hasher, IJwtService jwt)
    {
        _users = users;
        _hasher = hasher;
        _jwt = jwt;
    }

    public async Task Register(string email, string password, UserRole role, string name = "", string? phone = null)
    {
        var existing = await _users.GetByEmail(email);
        if (existing != null)
            throw new Exception("User already exists");

        var hash = _hasher.Hash(password);
        var user = new User(email, hash, role, name, phone);

        await _users.AddAsync(user);
    }

    public async Task<string> Login(string email, string password)
    {
        var users = await _users.GetByEmail(email);
        if (users == null)
            throw new Exception("Invalid credentials");

        var ok = _hasher.Verify(password, users.PasswordHash);
        if (!ok)
            throw new Exception("Invalid credentials");

        return _jwt.Generate(users.Id, users.Role.ToString(), users.Email);
    }
}