using KresApp.Application.DTOs;
using KresApp.Application.Interfaces;
using KresApp.Domain.Entities;
using KresApp.Domain.Enums;

namespace KresApp.Application.Services;

public class UserService
{
    private readonly IUserRepository _repo;
    private readonly IPasswordHasher _hasher;

    public UserService(IUserRepository repo, IPasswordHasher hasher)
    {
        _repo = repo;
        _hasher = hasher;
    }

    public async Task<IEnumerable<UserListDto>> GetAll(UserRole? role = null)
    {
        var users = role.HasValue 
            ? await _repo.GetByRoleAsync(role.Value)
            : await _repo.GetAllAsync();

        return users.Select(u => new UserListDto
        {
            Id = u.Id,
            Name = u.Name,
            Email = u.Email,
            Phone = u.Phone,
            Role = u.Role
        });
    }

    public async Task Create(CreateUserDto dto)
    {
        var existing = await _repo.GetByEmail(dto.Email);
        if (existing != null) throw new Exception("Bu e-posta adresi zaten kullanımda.");

        var hash = _hasher.Hash(dto.Password);
        var user = new User(dto.Email, hash, dto.Role, dto.Name, dto.Phone);
        
        await _repo.AddAsync(user);
    }

    public async Task Delete(Guid id)
    {
        await _repo.DeleteAsync(id);
    }
}
