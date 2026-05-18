using KresApp.Domain.Entities;

namespace KresApp.Application.Interfaces;

public interface ILeaveRequestRepository
{
    Task<List<LeaveRequest>> GetAllAsync();
    Task<List<LeaveRequest>> GetByChildIdAsync(Guid childId);
    Task<List<LeaveRequest>> GetPendingByClassIdAsync(Guid classId);
    Task<LeaveRequest?> GetByIdAsync(Guid id);
    Task AddAsync(LeaveRequest request);
    Task UpdateAsync(LeaveRequest request);
}
