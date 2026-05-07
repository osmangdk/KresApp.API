using KresApp.Application.Interfaces;
using KresApp.Domain.Entities;
using KresApp.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace KresApp.Persistence.Repositories;

public class UserAccessRequestRepository : IUserAccessRequestRepository
{
    private readonly AppDbContext _db;
    public UserAccessRequestRepository(AppDbContext db) => _db = db;

    public async Task<List<UserAccessRequest>> GetAllPendingAsync()
        => await _db.UserAccessRequests
            .Where(x => x.Status == AccessRequestStatus.Pending)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

    public async Task<UserAccessRequest?> GetByIdAsync(Guid id)
        => await _db.UserAccessRequests.FindAsync(id);

    public async Task<UserAccessRequest?> GetByEmailAsync(string email)
        => await _db.UserAccessRequests
            .Where(x => x.Email == email)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync();

    public async Task AddAsync(UserAccessRequest request)
    {
        await _db.UserAccessRequests.AddAsync(request);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(UserAccessRequest request)
    {
        _db.UserAccessRequests.Update(request);
        await _db.SaveChangesAsync();
    }
}
