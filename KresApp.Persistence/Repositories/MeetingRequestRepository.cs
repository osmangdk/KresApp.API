using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using KresApp.Application.Interfaces;
using KresApp.Domain.Entities;
using KresApp.Persistence.Context;

namespace KresApp.Persistence.Repositories;

public class MeetingRequestRepository : IMeetingRequestRepository
{
    private readonly AppDbContext _db;
    public MeetingRequestRepository(AppDbContext db) => _db = db;

    public async Task<List<MeetingRequest>> GetAllAsync()
        => await _db.MeetingRequests
            .Include(x => x.Child)
            .Include(x => x.Parent)
            .Include(x => x.Teacher)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

    public async Task<List<MeetingRequest>> GetByParentIdAsync(Guid parentId)
        => await _db.MeetingRequests
            .Include(x => x.Child)
            .Include(x => x.Parent)
            .Include(x => x.Teacher)
            .Where(x => x.ParentId == parentId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

    public async Task<List<MeetingRequest>> GetByTeacherIdAsync(Guid teacherId)
        => await _db.MeetingRequests
            .Include(x => x.Child)
            .Include(x => x.Parent)
            .Include(x => x.Teacher)
            .Where(x => x.TeacherId == teacherId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

    public async Task<List<MeetingRequest>> GetByChildIdAsync(Guid childId)
        => await _db.MeetingRequests
            .Include(x => x.Child)
            .Include(x => x.Parent)
            .Include(x => x.Teacher)
            .Where(x => x.ChildId == childId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

    public async Task<MeetingRequest?> GetByIdAsync(Guid id)
        => await _db.MeetingRequests
            .Include(x => x.Child)
            .Include(x => x.Parent)
            .Include(x => x.Teacher)
            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task AddAsync(MeetingRequest request)
    {
        await _db.MeetingRequests.AddAsync(request);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(MeetingRequest request)
    {
        _db.MeetingRequests.Update(request);
        await _db.SaveChangesAsync();
    }
}
