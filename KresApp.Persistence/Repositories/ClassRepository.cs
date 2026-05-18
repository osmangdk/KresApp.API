using Microsoft.EntityFrameworkCore;
using KresApp.Application.Interfaces;
using KresApp.Domain.Entities;
using KresApp.Persistence.Context;

namespace KresApp.Persistence.Repositories;

public class ClassRepository : IClassRepository
{
    private readonly AppDbContext _db;

    public ClassRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Class>> GetAllAsync()
    {
        return await _db.Classes.Include(x => x.AgeGroup).OrderBy(x => x.Name).ToListAsync();
    }

    public async Task<Class?> GetByIdAsync(Guid id)
    {
        return await _db.Classes.Include(x => x.AgeGroup).FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task AddAsync(Class classEntity)
    {
        await _db.Classes.AddAsync(classEntity);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Class classEntity)
    {
        _db.Classes.Update(classEntity);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Class classEntity)
    {
        _db.Classes.Remove(classEntity);
        await _db.SaveChangesAsync();
    }
}
