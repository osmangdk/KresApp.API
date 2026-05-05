using KresApp.Domain.Entities;

namespace KresApp.Application.Interfaces;

public interface IClassRepository
{
    Task<List<Class>> GetAllAsync();
    Task<Class?> GetByIdAsync(Guid id);
    Task AddAsync(Class classEntity);
    Task UpdateAsync(Class classEntity);
    Task DeleteAsync(Class classEntity);
}
