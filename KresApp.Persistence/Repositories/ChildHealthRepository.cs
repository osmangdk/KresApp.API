using KresApp.Application.Interfaces;
using KresApp.Domain.Entities;
using KresApp.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KresApp.Persistence.Repositories;

public class ChildHealthRepository : IChildHealthRepository
{
    private readonly AppDbContext _db;
    public ChildHealthRepository(AppDbContext db) => _db = db;

    public async Task<List<ChildHealthRecord>> GetByChildIdAsync(Guid childId)
    {
        return await _db.ChildHealthRecords
            .Where(x => x.ChildId == childId)
            .OrderBy(x => x.Category)
            .OrderByDescending(x => x.OccurrenceDate)
            .ToListAsync();
    }

    public async Task<ChildHealthRecord?> GetByIdAsync(Guid id)
    {
        return await _db.ChildHealthRecords.FindAsync(id);
    }

    public async Task AddAsync(ChildHealthRecord record)
    {
        await _db.ChildHealthRecords.AddAsync(record);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(ChildHealthRecord record)
    {
        _db.ChildHealthRecords.Update(record);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(ChildHealthRecord record)
    {
        _db.ChildHealthRecords.Remove(record);
        await _db.SaveChangesAsync();
    }

    public async Task AddRangeAsync(IEnumerable<ChildHealthRecord> records)
    {
        await _db.ChildHealthRecords.AddRangeAsync(records);
        await _db.SaveChangesAsync();
    }
}
