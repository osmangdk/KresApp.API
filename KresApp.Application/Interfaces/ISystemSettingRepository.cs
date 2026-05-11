using KresApp.Domain.Entities;

namespace KresApp.Application.Interfaces;

public interface ISystemSettingRepository
{
    Task<SystemSetting?> GetAsync();
    Task AddAsync(SystemSetting setting);
    Task UpdateAsync(SystemSetting setting);
}
