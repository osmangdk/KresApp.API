using KresApp.Application.Interfaces;
using KresApp.Domain.Entities;
using KresApp.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace KresApp.Persistence.Repositories;

public class SystemSettingRepository : ISystemSettingRepository
{
    private readonly AppDbContext _context;

    public SystemSettingRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<SystemSetting?> GetAsync()
    {
        return await _context.SystemSettings.FirstOrDefaultAsync();
    }

    public async Task AddAsync(SystemSetting setting)
    {
        _context.SystemSettings.Add(setting);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(SystemSetting setting)
    {
        _context.SystemSettings.Update(setting);
        await _context.SaveChangesAsync();
    }
}
