using Microsoft.EntityFrameworkCore;
using KresApp.Application.Interfaces;
using KresApp.Domain.Entities;
using KresApp.Persistence.Context;

namespace KresApp.Persistence.Repositories;

public class ChildAllergyRepository : IChildAllergyRepository
{
    private readonly AppDbContext _db;

    public ChildAllergyRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<ChildAllergy>> GetByChildIdAsync(Guid childId)
    {
        return await _db.ChildAllergies
            .Where(x => x.ChildId == childId)
            .OrderBy(x => x.AllergyName)
            .ToListAsync();
    }

    public async Task<ChildAllergy?> GetByIdAsync(Guid id)
    {
        return await _db.ChildAllergies.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task AddAsync(ChildAllergy allergy)
    {
        await _db.ChildAllergies.AddAsync(allergy);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(ChildAllergy allergy)
    {
        _db.ChildAllergies.Update(allergy);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(ChildAllergy allergy)
    {
        _db.ChildAllergies.Remove(allergy);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAllByChildIdAsync(Guid childId)
    {
        var allergies = await _db.ChildAllergies
            .Where(x => x.ChildId == childId)
            .ToListAsync();

        _db.ChildAllergies.RemoveRange(allergies);
        await _db.SaveChangesAsync();
    }
}
