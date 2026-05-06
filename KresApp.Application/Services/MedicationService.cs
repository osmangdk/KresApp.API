using KresApp.Application.DTOs;
using KresApp.Application.Interfaces;
using KresApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KresApp.Application.Services;

public class MedicationService
{
    private readonly IMedicationRepository _repo;

    public MedicationService(IMedicationRepository repo) => _repo = repo;

    public async Task<List<MedicationDto>> GetByChildIdAsync(Guid childId, DateTime date)
    {
        var medications = await _repo.GetByChildIdAsync(childId);
        var logs = await _repo.GetLogsAsync(childId, date.Date);

        return medications.Select(m => new MedicationDto
        {
            Id = m.Id,
            ChildId = m.ChildId,
            Name = m.Name,
            Dosage = m.Dosage,
            Times = m.Times,
            Note = m.Note,
            StartDate = m.StartDate,
            EndDate = m.EndDate,
            IsActive = m.IsActive,
            TodayLogs = m.Times.Select(time => {
                var log = logs.FirstOrDefault(l => l.MedicationId == m.Id && l.Time == time);
                return new MedicationLogDto
                {
                    Id = log?.Id,
                    Time = time,
                    IsGiven = log?.IsGiven ?? false,
                    GivenByUserId = log?.GivenByUserId,
                    GivenByName = log?.GivenBy?.Name,
                    GivenAt = log?.GivenAt
                };
            }).ToList()
        }).ToList();
    }

    public async Task<MedicationDto?> GetByIdAsync(Guid id)
    {
        var m = await _repo.GetByIdAsync(id);
        if (m == null) return null;

        return new MedicationDto
        {
            Id = m.Id,
            ChildId = m.ChildId,
            Name = m.Name,
            Dosage = m.Dosage,
            Times = m.Times,
            Note = m.Note,
            StartDate = m.StartDate,
            EndDate = m.EndDate,
            IsActive = m.IsActive
        };
    }

    public async Task CreateAsync(CreateMedicationDto dto)
    {
        var medication = new Medication
        {
            Id = Guid.NewGuid(),
            ChildId = dto.ChildId,
            Name = dto.Name,
            Dosage = dto.Dosage,
            Times = dto.Times,
            Note = dto.Note,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(medication);
    }

    public async Task GiveAsync(GiveMedicationDto dto, Guid userId)
    {
        var log = await _repo.GetLogAsync(dto.MedicationId, dto.Date.Date, dto.Time);
        if (log == null)
        {
            log = new MedicationLog
            {
                Id = Guid.NewGuid(),
                MedicationId = dto.MedicationId,
                Date = dto.Date.Date,
                Time = dto.Time,
                IsGiven = true,
                GivenByUserId = userId,
                GivenAt = DateTime.UtcNow
            };
            await _repo.AddLogAsync(log);
        }
        else
        {
            log.IsGiven = true;
            log.GivenByUserId = userId;
            log.GivenAt = DateTime.UtcNow;
            await _repo.UpdateLogAsync(log);
        }
    }

    public async Task ToggleActiveAsync(Guid id)
    {
        var m = await _repo.GetByIdAsync(id);
        if (m != null)
        {
            m.IsActive = !m.IsActive;
            await _repo.UpdateAsync(m);
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        var m = await _repo.GetByIdAsync(id);
        if (m != null) await _repo.DeleteAsync(m);
    }
}
