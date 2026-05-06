using KresApp.Application.Interfaces;
using KresApp.Domain.Entities;
using KresApp.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KresApp.Persistence.Repositories;

public class MedicationRepository : IMedicationRepository
{
    private readonly AppDbContext _db;
    public MedicationRepository(AppDbContext db) => _db = db;

    public async Task<List<Medication>> GetByChildIdAsync(Guid childId)
    {
        return await _db.Medications
            .Where(x => x.ChildId == childId && x.IsActive)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<Medication?> GetByIdAsync(Guid id)
    {
        return await _db.Medications.FindAsync(id);
    }

    public async Task AddAsync(Medication medication)
    {
        await _db.Medications.AddAsync(medication);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Medication medication)
    {
        _db.Medications.Update(medication);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Medication medication)
    {
        _db.Medications.Remove(medication);
        await _db.SaveChangesAsync();
    }

    public async Task<List<MedicationLog>> GetLogsAsync(Guid childId, DateTime date)
    {
        return await _db.MedicationLogs
            .Include(x => x.GivenBy)
            .Where(x => x.Medication.ChildId == childId && x.Date == date.Date)
            .ToListAsync();
    }

    public async Task<MedicationLog?> GetLogAsync(Guid medicationId, DateTime date, string time)
    {
        return await _db.MedicationLogs
            .FirstOrDefaultAsync(x => x.MedicationId == medicationId && x.Date == date.Date && x.Time == time);
    }

    public async Task AddLogAsync(MedicationLog log)
    {
        await _db.MedicationLogs.AddAsync(log);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateLogAsync(MedicationLog log)
    {
        _db.MedicationLogs.Update(log);
        await _db.SaveChangesAsync();
    }
}
