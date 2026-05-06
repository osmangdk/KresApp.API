using KresApp.Application.Interfaces;
using KresApp.Domain.Entities;
using KresApp.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KresApp.Persistence.Repositories;

public class VaccinationRepository : IVaccinationRepository
{
    private readonly AppDbContext _db;
    public VaccinationRepository(AppDbContext db) => _db = db;

    public async Task<List<Vaccination>> GetByChildIdAsync(Guid childId)
    {
        return await _db.Vaccinations
            .Where(x => x.ChildId == childId)
            .OrderByDescending(x => x.ScheduledDate)
            .OrderByDescending(x => x.DateAdministered)
            .ToListAsync();
    }

    public async Task<Vaccination?> GetByIdAsync(Guid id)
    {
        return await _db.Vaccinations.FindAsync(id);
    }

    public async Task AddAsync(Vaccination vaccination)
    {
        await _db.Vaccinations.AddAsync(vaccination);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Vaccination vaccination)
    {
        _db.Vaccinations.Update(vaccination);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Vaccination vaccination)
    {
        _db.Vaccinations.Remove(vaccination);
        await _db.SaveChangesAsync();
    }
}
