using KresApp.Domain.Entities;

namespace KresApp.Application.Interfaces;

public interface IChildRepository
{
    Task<List<Child>> GetByFilter(Guid userId, string role);
    Task<Child?> GetByIdAsync(Guid id);
    Task<List<Child>> SearchAsync(string? q, Guid? classId);
    Task AddAsync(Child child);
    Task UpdateAsync(Child child);
    Task DeleteAsync(Child child);
}