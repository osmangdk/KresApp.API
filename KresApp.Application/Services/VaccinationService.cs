using KresApp.Application.DTOs;
using KresApp.Application.Interfaces;
using KresApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KresApp.Application.Services;

public class VaccinationService
{
    private readonly IVaccinationRepository _repo;

    public VaccinationService(IVaccinationRepository repo) => _repo = repo;

    public async Task<List<VaccinationDto>> GetByChildIdAsync(Guid childId)
    {
        var vaccinations = await _repo.GetByChildIdAsync(childId);
        return vaccinations.Select(v => new VaccinationDto
        {
            Id = v.Id,
            ChildId = v.ChildId,
            VaccineName = v.VaccineName,
            DateAdministered = v.DateAdministered,
            ScheduledDate = v.ScheduledDate,
            Status = v.Status,
            Dose = v.Dose,
            Notes = v.Notes
        }).ToList();
    }

    public async Task CreateAsync(CreateVaccinationDto dto)
    {
        var vaccination = new Vaccination
        {
            Id = Guid.NewGuid(),
            ChildId = dto.ChildId,
            VaccineName = dto.VaccineName,
            DateAdministered = dto.DateAdministered,
            ScheduledDate = dto.ScheduledDate,
            Status = dto.Status,
            Dose = dto.Dose,
            Notes = dto.Notes,
            CreatedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(vaccination);
    }

    public async Task UpdateAsync(UpdateVaccinationDto dto)
    {
        var v = await _repo.GetByIdAsync(dto.Id);
        if (v != null)
        {
            v.VaccineName = dto.VaccineName;
            v.DateAdministered = dto.DateAdministered;
            v.ScheduledDate = dto.ScheduledDate;
            v.Status = dto.Status;
            v.Dose = dto.Dose;
            v.Notes = dto.Notes;

            await _repo.UpdateAsync(v);
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        var v = await _repo.GetByIdAsync(id);
        if (v != null) await _repo.DeleteAsync(v);
    }
}
