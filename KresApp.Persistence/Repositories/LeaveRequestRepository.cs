using Microsoft.EntityFrameworkCore;
using KresApp.Application.Interfaces;
using KresApp.Domain.Entities;
using KresApp.Persistence.Context;

namespace KresApp.Persistence.Repositories;

public class LeaveRequestRepository : ILeaveRequestRepository
{
    private readonly AppDbContext _db;
    public LeaveRequestRepository(AppDbContext db) => _db = db;

    public async Task<List<LeaveRequest>> GetAllAsync()
        => await _db.LeaveRequests.Include(x => x.Child).OrderByDescending(x => x.CreatedAt).ToListAsync();

    public async Task<List<LeaveRequest>> GetByChildIdAsync(Guid childId)
        => await _db.LeaveRequests.Where(x => x.ChildId == childId).OrderByDescending(x => x.CreatedAt).ToListAsync();

    public async Task<List<LeaveRequest>> GetPendingByClassIdAsync(Guid classId)
        => await _db.LeaveRequests
            .Include(x => x.Child)
            .Where(x => x.Child.ClassId == classId && x.Status == Domain.Enums.LeaveStatus.Pending)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

    public async Task<LeaveRequest?> GetByIdAsync(Guid id)
        => await _db.LeaveRequests.Include(x => x.Child).FirstOrDefaultAsync(x => x.Id == id);

    public async Task AddAsync(LeaveRequest request)
    {
        await _db.LeaveRequests.AddAsync(request);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(LeaveRequest request)
    {
        _db.LeaveRequests.Update(request);
        await _db.SaveChangesAsync();
    }
}
