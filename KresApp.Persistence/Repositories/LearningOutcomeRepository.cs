using Microsoft.EntityFrameworkCore;
using KresApp.Application.Interfaces;
using KresApp.Domain.Entities;
using KresApp.Persistence.Context;

namespace KresApp.Persistence.Repositories;

public class LearningOutcomeRepository : ILearningOutcomeRepository
{
    private readonly AppDbContext _db;
    public LearningOutcomeRepository(AppDbContext db) => _db = db;

    public async Task<List<LearningOutcome>> GetAllAsync(int? year = null)
    {
        var query = _db.LearningOutcomes.AsQueryable();
        if (year.HasValue)
            query = query.Where(x => x.Year == year.Value);
        return await query.OrderBy(x => x.Year).ThenBy(x => x.Month).ToListAsync();
    }

    public async Task<LearningOutcome?> GetByMonthYearAsync(int month, int year)
        => await _db.LearningOutcomes.FirstOrDefaultAsync(x => x.Month == month && x.Year == year);

    public async Task<LearningOutcome?> GetByIdAsync(Guid id)
        => await _db.LearningOutcomes.FindAsync(id);

    public async Task AddAsync(LearningOutcome outcome)
    {
        await _db.LearningOutcomes.AddAsync(outcome);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(LearningOutcome outcome)
    {
        _db.LearningOutcomes.Update(outcome);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(LearningOutcome outcome)
    {
        _db.LearningOutcomes.Remove(outcome);
        await _db.SaveChangesAsync();
    }
}
