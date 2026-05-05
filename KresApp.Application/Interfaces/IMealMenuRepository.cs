using KresApp.Domain.Entities;

namespace KresApp.Application.Interfaces;

public interface IMealMenuRepository
{
    Task<List<MealMenu>> GetAllAsync();
    Task<MealMenu?> GetByIdAsync(Guid id);
    Task AddAsync(MealMenu menu);
    Task UpdateAsync(MealMenu menu);
    Task DeleteAsync(MealMenu menu);
}
