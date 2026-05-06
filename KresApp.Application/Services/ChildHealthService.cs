using KresApp.Application.DTOs;
using KresApp.Application.Interfaces;
using KresApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KresApp.Application.Services;

public class ChildHealthService
{
    private readonly IChildHealthRepository _repo;
    private readonly IChildRepository _childRepo;

    public ChildHealthService(IChildHealthRepository repo, IChildRepository childRepo)
    {
        _repo = repo;
        _childRepo = childRepo;
    }

    public async Task<ChildHealthHistoryDto?> GetHistoryByChildIdAsync(Guid childId)
    {
        var child = await _childRepo.GetByIdAsync(childId);
        if (child == null) return null;

        var records = await _repo.GetByChildIdAsync(childId);

        return new ChildHealthHistoryDto
        {
            ChildId = child.Id,
            ParentId = child.ParentId,
            ChildName = child.Name,
            Gender = child.Gender,
            BirthDate = child.BirthDate,
            MedicalNotes = child.MedicalNotes,
            ContagiousDiseases = records
                .Where(x => x.Category == "Contagious")
                .Select(MapToDto)
                .ToList(),
            ChronicDiseases = records
                .Where(x => x.Category == "Chronic")
                .Select(MapToDto)
                .ToList()
        };
    }

    public async Task CreateRecordAsync(CreateChildHealthRecordDto dto)
    {
        var record = new ChildHealthRecord
        {
            Id = Guid.NewGuid(),
            ChildId = dto.ChildId,
            DiseaseName = dto.DiseaseName,
            OccurrenceDate = dto.OccurrenceDate,
            Description = dto.Description,
            Category = dto.Category,
            CreatedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(record);
    }

    public async Task UpdateRecordAsync(UpdateChildHealthRecordDto dto)
    {
        var record = await _repo.GetByIdAsync(dto.Id);
        if (record != null)
        {
            record.DiseaseName = dto.DiseaseName;
            record.OccurrenceDate = dto.OccurrenceDate;
            record.Description = dto.Description;
            record.Category = dto.Category;

            await _repo.UpdateAsync(record);
        }
    }

    public async Task DeleteRecordAsync(Guid id)
    {
        var record = await _repo.GetByIdAsync(id);
        if (record != null) await _repo.DeleteAsync(record);
    }

    public async Task<ChildHealthHistoryDto?> GetHistoryByRecordIdAsync(Guid recordId)
    {
        var record = await _repo.GetByIdAsync(recordId);
        if (record == null) return null;
        return await GetHistoryByChildIdAsync(record.ChildId);
    }

    private ChildHealthRecordDto MapToDto(ChildHealthRecord record)
    {
        return new ChildHealthRecordDto
        {
            Id = record.Id,
            ChildId = record.ChildId,
            DiseaseName = record.DiseaseName,
            OccurrenceDate = record.OccurrenceDate,
            Description = record.Description,
            Category = record.Category
        };
    }
}
