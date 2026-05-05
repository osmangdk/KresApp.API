using Microsoft.EntityFrameworkCore;
using KresApp.Application.Interfaces;
using KresApp.Domain.Entities;
using KresApp.Persistence.Context;

namespace KresApp.Persistence.Repositories;

public class DailyReportRepository : IDailyReportRepository
{
    private readonly AppDbContext _db;

    public DailyReportRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<DailyReport>> GetByDateAsync(DateOnly date, Guid? classId = null)
    {
        var query = _db.DailyReports.Where(x => x.Date == date);

        if (classId.HasValue)
        {
            var childIds = await _db.Children.Where(c => c.ClassId == classId.Value).Select(c => c.Id).ToListAsync();
            query = query.Where(x => childIds.Contains(x.ChildId));
        }

        return await query.ToListAsync();
    }

    public async Task<DailyReport?> GetByChildAndDateAsync(Guid childId, DateOnly date)
    {
        return await _db.DailyReports.FirstOrDefaultAsync(x => x.ChildId == childId && x.Date == date);
    }

    public async Task<DailyReport?> GetByIdAsync(Guid id)
    {
        return await _db.DailyReports.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task AddAsync(DailyReport report)
    {
        await _db.DailyReports.AddAsync(report);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(DailyReport report)
    {
        _db.DailyReports.Update(report);
        await _db.SaveChangesAsync();
    }
}
