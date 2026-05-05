using Microsoft.EntityFrameworkCore;
using KresApp.Application.Interfaces;
using KresApp.Domain.Entities;
using KresApp.Persistence.Context;

namespace KresApp.Persistence.Repositories;

public class ChildRepository : IChildRepository
{
    private readonly AppDbContext _db;

    public ChildRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Child>> GetByFilter(Guid userId, string role)
    {
        var query = _db.Children.AsQueryable();

        if (role == "Parent")
            query = query.Where(x => x.ParentId == userId);

        if (role == "Teacher")
        {
            var classIds = await _db.Classes
                .Where(c => c.TeacherId == userId)
                .Select(c => c.Id)
                .ToListAsync();

            query = query.Where(x => classIds.Contains(x.ClassId));
        }

        return await query.ToListAsync();
    }

    public async Task AddAsync(Child child)
    {
        await _db.Children.AddAsync(child);
        await _db.SaveChangesAsync();
    }

    public async Task<Child?> GetByIdAsync(Guid id)
    {
        return await _db.Children.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<Child>> SearchAsync(string? q, Guid? classId)
    {
        var query = _db.Children.AsQueryable();

        if (!string.IsNullOrEmpty(q))
            query = query.Where(x => x.Name.ToLower().Contains(q.ToLower()));

        if (classId.HasValue)
            query = query.Where(x => x.ClassId == classId.Value);

        return await query.ToListAsync();
    }

    public async Task UpdateAsync(Child child)
    {
        _db.Children.Update(child);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Child child)
    {
        _db.Children.Remove(child);
        await _db.SaveChangesAsync();
    }
}