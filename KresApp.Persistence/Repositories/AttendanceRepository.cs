using Microsoft.EntityFrameworkCore;
using KresApp.Application.Interfaces;
using KresApp.Domain.Entities;
using KresApp.Persistence.Context;

namespace KresApp.Persistence.Repositories;

public class AttendanceRepository : IAttendanceRepository
{
    private readonly AppDbContext _db;

    public AttendanceRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Attendance>> GetByDateAsync(DateOnly date, Guid? classId = null)
    {
        var query = _db.Attendances.Where(x => x.Date == date);
        
        if (classId.HasValue)
        {
            var childIds = await _db.Children.Where(c => c.ClassId == classId.Value).Select(c => c.Id).ToListAsync();
            query = query.Where(x => childIds.Contains(x.ChildId));
        }

        return await query.ToListAsync();
    }

    public async Task<List<Attendance>> GetByDateRangeAsync(DateOnly startDate, DateOnly endDate)
    {
        return await _db.Attendances
            .Where(x => x.Date >= startDate && x.Date <= endDate)
            .ToListAsync();
    }

    public async Task<List<Attendance>> GetByChildAsync(Guid childId, DateOnly? startDate = null, DateOnly? endDate = null)
    {
        var query = _db.Attendances.Where(x => x.ChildId == childId);
        
        if (startDate.HasValue) query = query.Where(x => x.Date >= startDate.Value);
        if (endDate.HasValue) query = query.Where(x => x.Date <= endDate.Value);

        return await query.OrderByDescending(x => x.Date).ToListAsync();
    }

    public async Task AddRangeAsync(IEnumerable<Attendance> records)
    {
        await _db.Attendances.AddRangeAsync(records);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Attendance attendance)
    {
        _db.Attendances.Update(attendance);
        await _db.SaveChangesAsync();
    }

    public async Task<Attendance?> GetByIdAsync(Guid id)
    {
        return await _db.Attendances.FirstOrDefaultAsync(x => x.Id == id);
    }
}
