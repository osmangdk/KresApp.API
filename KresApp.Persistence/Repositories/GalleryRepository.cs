using KresApp.Application.Interfaces;
using KresApp.Domain.Entities;
using KresApp.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KresApp.Persistence.Repositories;

public class GalleryRepository : IGalleryRepository
{
    private readonly AppDbContext _db;
    public GalleryRepository(AppDbContext db) => _db = db;

    public async Task<List<GalleryItem>> GetByClassIdAsync(Guid classId)
    {
        return await _db.GalleryItems
            .Include(x => x.CreatedBy)
            .Where(x => x.ClassId == classId)
            .ToListAsync();
    }

    public async Task<List<GalleryItem>> GetByChildIdAsync(Guid childId)
    {
        return await _db.GalleryItems
            .Include(x => x.CreatedBy)
            .Where(x => x.ChildId == childId)
            .ToListAsync();
    }

    public async Task<List<GalleryItem>> GetAllAsync()
    {
        return await _db.GalleryItems
            .Include(x => x.CreatedBy)
            .ToListAsync();
    }

    public async Task<GalleryItem?> GetByIdAsync(Guid id)
    {
        return await _db.GalleryItems.FindAsync(id);
    }

    public async Task AddAsync(GalleryItem item)
    {
        await _db.GalleryItems.AddAsync(item);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(GalleryItem item)
    {
        _db.GalleryItems.Remove(item);
        await _db.SaveChangesAsync();
    }
}
