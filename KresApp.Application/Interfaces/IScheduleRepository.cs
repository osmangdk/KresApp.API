using KresApp.Domain.Entities;

namespace KresApp.Application.Interfaces;

public interface IScheduleRepository
{
    Task<List<Schedule>> GetAllAsync();
    Task<Schedule?> GetByIdAsync(Guid id);
    Task AddAsync(Schedule schedule);
    Task UpdateAsync(Schedule schedule);
    Task DeleteAsync(Schedule schedule);
}
