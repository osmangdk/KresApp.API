using KresApp.Domain.Entities;

namespace KresApp.Application.Interfaces;

public interface IDailyReportRepository
{
    Task<List<DailyReport>> GetByDateAsync(DateOnly date, Guid? classId = null);
    Task<DailyReport?> GetByChildAndDateAsync(Guid childId, DateOnly date);
    Task<DailyReport?> GetByIdAsync(Guid id);
    Task AddAsync(DailyReport report);
    Task UpdateAsync(DailyReport report);
}
