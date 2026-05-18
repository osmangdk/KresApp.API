using Microsoft.EntityFrameworkCore;
using KresApp.Application.Interfaces;
using KresApp.Domain.Entities;
using KresApp.Persistence.Context;

namespace KresApp.Persistence.Repositories;

public class AgeGroupRepository : IAgeGroupRepository
{
    private readonly AppDbContext _db;

    public AgeGroupRepository(AppDbContext db) => _db = db;

    public async Task<List<AgeGroup>> GetAllAsync()
    {
        return await _db.AgeGroups
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<AgeGroup?> GetByIdAsync(Guid id)
    {
        return await _db.AgeGroups.FindAsync(id);
    }

    public async Task AddAsync(AgeGroup entity)
    {
        await _db.AgeGroups.AddAsync(entity);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(AgeGroup entity)
    {
        _db.AgeGroups.Update(entity);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(AgeGroup entity)
    {
        _db.AgeGroups.Remove(entity);
        await _db.SaveChangesAsync();
    }
}
