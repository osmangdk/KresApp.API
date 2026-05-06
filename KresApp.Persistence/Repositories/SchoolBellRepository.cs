using Microsoft.EntityFrameworkCore;
using KresApp.Application.Interfaces;
using KresApp.Domain.Entities;
using KresApp.Persistence.Context;

namespace KresApp.Persistence.Repositories;

public class SchoolBellRepository : ISchoolBellRepository
{
    private readonly AppDbContext _db;

    public SchoolBellRepository(AppDbContext db) => _db = db;

    public async Task<List<SchoolBellRequest>> GetAllAsync(DateTime? date = null)
    {
        var query = _db.SchoolBellRequests
            .Include(x => x.Child)
            .Include(x => x.Parent)
            .AsQueryable();

        if (date.HasValue)
        {
            var targetDate = date.Value.Date;
            query = query.Where(x => x.CreatedAt.Date == targetDate);
        }

        return await query.OrderByDescending(x => x.CreatedAt).ToListAsync();
    }

    public async Task<List<SchoolBellRequest>> GetByParentAsync(Guid parentId)
    {
        return await _db.SchoolBellRequests
            .Include(x => x.Child)
            .Where(x => x.ParentId == parentId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<SchoolBellRequest?> GetByIdAsync(Guid id)
    {
        return await _db.SchoolBellRequests
            .Include(x => x.Child)
            .Include(x => x.Parent)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task AddAsync(SchoolBellRequest request)
    {
        await _db.SchoolBellRequests.AddAsync(request);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(SchoolBellRequest request)
    {
        _db.SchoolBellRequests.Update(request);
        await _db.SaveChangesAsync();
    }
}
