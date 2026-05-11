using KresApp.Application.Interfaces;
using KresApp.Domain.Entities;
using KresApp.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace KresApp.Persistence.Repositories;

public class EnrollmentRequestRepository : IEnrollmentRequestRepository
{
    private readonly AppDbContext _db;
    public EnrollmentRequestRepository(AppDbContext db) => _db = db;

    public async Task<List<EnrollmentRequest>> GetAllAsync()
        => await _db.EnrollmentRequests
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

    public async Task<List<EnrollmentRequest>> GetPendingAsync()
        => await _db.EnrollmentRequests
            .Where(x => x.Status == EnrollmentStatus.Pending)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

    public async Task<EnrollmentRequest?> GetByIdAsync(Guid id)
        => await _db.EnrollmentRequests.FindAsync(id);

    public async Task<EnrollmentRequest?> GetByEmailAsync(string email)
        => await _db.EnrollmentRequests
            .Where(x => x.ParentEmail == email)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync();

    public async Task AddAsync(EnrollmentRequest request)
    {
        await _db.EnrollmentRequests.AddAsync(request);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(EnrollmentRequest request)
    {
        _db.EnrollmentRequests.Update(request);
        await _db.SaveChangesAsync();
    }
}
