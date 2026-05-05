using KresApp.Application.DTOs;
using KresApp.Application.Interfaces;

namespace KresApp.Application.Services;

public class ProfileService
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _hasher;

    public ProfileService(IUserRepository users, IPasswordHasher hasher)
    {
        _users = users;
        _hasher = hasher;
    }

    public async Task<ProfileDto> GetProfileAsync(string email)
    {
        var user = await _users.GetByEmail(email);
        if (user == null) throw new Exception("User not found");

        return new ProfileDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Phone = user.Phone,
            Role = user.Role.ToString()
        };
    }

    public async Task UpdateProfileAsync(string email, UpdateProfileDto dto)
    {
        var user = await _users.GetByEmail(email);
        if (user == null) throw new Exception("User not found");

        user.UpdateProfile(dto.Name, dto.Phone);
        await _users.UpdateAsync(user);
    }

    public async Task ChangePasswordAsync(string email, ChangePasswordDto dto)
    {
        var user = await _users.GetByEmail(email);
        if (user == null) throw new Exception("User not found");

        if (!_hasher.Verify(dto.CurrentPassword, user.PasswordHash))
            throw new Exception("Invalid current password");

        var newHash = _hasher.Hash(dto.NewPassword);
        user.ChangePassword(newHash);
        
        await _users.UpdateAsync(user);
    }
}
