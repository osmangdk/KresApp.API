using KresApp.Domain.Entities;

using KresApp.Domain.Enums;

namespace KresApp.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmail(string email);
    Task<IEnumerable<User>> GetByRoleAsync(UserRole role);
    Task AddAsync(User user);
    Task UpdateAsync(User user);
}