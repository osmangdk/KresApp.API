using KresApp.Domain.Entities;

namespace KresApp.Application.Interfaces;

public interface IAttendanceRepository
{
    Task<List<Attendance>> GetByDateAsync(DateOnly date, Guid? classId = null);
    Task<List<Attendance>> GetByChildAsync(Guid childId, DateOnly? startDate = null, DateOnly? endDate = null);
    Task AddRangeAsync(IEnumerable<Attendance> records);
    Task UpdateAsync(Attendance attendance);
    Task<Attendance?> GetByIdAsync(Guid id);
}
