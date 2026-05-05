using Microsoft.EntityFrameworkCore;
using KresApp.Application.Interfaces;
using KresApp.Domain.Entities;
using KresApp.Domain.Enums;
using KresApp.Persistence.Context;

namespace KresApp.Persistence.Repositories;

public class AnnouncementRepository : IAnnouncementRepository
{
    private readonly AppDbContext _db;

    public AnnouncementRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Announcement>> GetAllAsync(AnnouncementCategory? category = null)
    {
        var query = _db.Announcements.AsQueryable();
        
        if (category.HasValue)
            query = query.Where(x => x.Category == category.Value);

        return await query.OrderByDescending(x => x.Date).ToListAsync();
    }

    public async Task<Announcement?> GetByIdAsync(Guid id)
    {
        return await _db.Announcements.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task AddAsync(Announcement announcement)
    {
        await _db.Announcements.AddAsync(announcement);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Announcement announcement)
    {
        _db.Announcements.Update(announcement);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Announcement announcement)
    {
        _db.Announcements.Remove(announcement);
        await _db.SaveChangesAsync();
    }
}
