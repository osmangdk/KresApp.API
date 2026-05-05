using Microsoft.EntityFrameworkCore;
using KresApp.Application.Interfaces;
using KresApp.Domain.Entities;
using KresApp.Persistence.Context;

namespace KresApp.Persistence.Repositories;

public class ScheduleRepository : IScheduleRepository
{
    private readonly AppDbContext _db;
    public ScheduleRepository(AppDbContext db) => _db = db;

    public async Task<List<Schedule>> GetAllAsync()
    {
        return await _db.Schedules.ToListAsync();
    }

    public async Task<Schedule?> GetByIdAsync(Guid id)
    {
        return await _db.Schedules.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task AddAsync(Schedule schedule)
    {
        await _db.Schedules.AddAsync(schedule);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Schedule schedule)
    {
        _db.Schedules.Update(schedule);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Schedule schedule)
    {
        _db.Schedules.Remove(schedule);
        await _db.SaveChangesAsync();
    }
}
