using KresApp.Application.DTOs;
using KresApp.Application.Interfaces;
using KresApp.Domain.Enums;

namespace KresApp.Application.Services;

public class UserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<UserListDto>> GetTeachersAsync()
    {
        var teachers = await _userRepository.GetByRoleAsync(UserRole.Teacher);
        
        return teachers.Select(t => new UserListDto
        {
            Id = t.Id,
            Name = t.Name,
            Email = t.Email,
            Phone = t.Phone,
            Role = t.Role
        });
    }
    public async Task<IEnumerable<UserListDto>> GetParentsAsync()
    {
        var parents = await _userRepository.GetByRoleAsync(UserRole.Parent);
        
        return parents.Select(t => new UserListDto
        {
            Id = t.Id,
            Name = t.Name,
            Email = t.Email,
            Phone = t.Phone,
            Role = t.Role
        });
    }
}
