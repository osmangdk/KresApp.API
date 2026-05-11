using KresApp.Application.DTOs;
using KresApp.Application.Interfaces;
using KresApp.Domain.Entities;

namespace KresApp.Application.Services;

public class SystemSettingService
{
    private readonly ISystemSettingRepository _repo;

    public SystemSettingService(ISystemSettingRepository repo)
    {
        _repo = repo;
    }

    public async Task<SystemSettingDto> GetSettingsAsync()
    {
        var settings = await GetInternalSettingsAsync();
        return new SystemSettingDto(
            settings.IsPreEnrollmentActive,
            settings.PreEnrollmentStartDate,
            settings.PreEnrollmentEndDate,
            settings.UpdatedAt
        );
    }

    private async Task<SystemSetting> GetInternalSettingsAsync()
    {
        var settings = await _repo.GetAsync();
        if (settings == null)
        {
            settings = new SystemSetting();
            await _repo.AddAsync(settings);
        }
        return settings;
    }

    public async Task UpdateSettingsAsync(UpdateSystemSettingDto dto)
    {
        var settings = await GetInternalSettingsAsync();
        settings.IsPreEnrollmentActive = dto.IsPreEnrollmentActive;
        settings.PreEnrollmentStartDate = dto.PreEnrollmentStartDate;
        settings.PreEnrollmentEndDate = dto.PreEnrollmentEndDate;
        settings.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(settings);
    }

    public async Task<bool> IsPreEnrollmentAllowedAsync()
    {
        var settings = await GetInternalSettingsAsync();
        if (!settings.IsPreEnrollmentActive) return false;

        var now = DateTime.UtcNow;
        if (settings.PreEnrollmentStartDate.HasValue && now < settings.PreEnrollmentStartDate.Value)
            return false;

        if (settings.PreEnrollmentEndDate.HasValue && now > settings.PreEnrollmentEndDate.Value)
            return false;

        return true;
    }
}
