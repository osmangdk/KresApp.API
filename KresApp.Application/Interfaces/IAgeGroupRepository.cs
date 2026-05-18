using KresApp.Domain.Entities;

namespace KresApp.Application.Interfaces;

public interface IAgeGroupRepository
{
    Task<List<AgeGroup>> GetAllAsync();
    Task<AgeGroup?> GetByIdAsync(Guid id);
    Task AddAsync(AgeGroup entity);
    Task UpdateAsync(AgeGroup entity);
    Task DeleteAsync(AgeGroup entity);
}
