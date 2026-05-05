using Microsoft.EntityFrameworkCore;
using KresApp.Application.Interfaces;
using KresApp.Domain.Entities;
using KresApp.Persistence.Context;

namespace KresApp.Persistence.Repositories;

public class MealMenuRepository : IMealMenuRepository
{
    private readonly AppDbContext _db;
    public MealMenuRepository(AppDbContext db) => _db = db;

    public async Task<List<MealMenu>> GetAllAsync()
    {
        return await _db.MealMenus.ToListAsync();
    }

    public async Task<MealMenu?> GetByIdAsync(Guid id)
    {
        return await _db.MealMenus.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task AddAsync(MealMenu menu)
    {
        await _db.MealMenus.AddAsync(menu);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(MealMenu menu)
    {
        _db.MealMenus.Update(menu);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(MealMenu menu)
    {
        _db.MealMenus.Remove(menu);
        await _db.SaveChangesAsync();
    }
}
