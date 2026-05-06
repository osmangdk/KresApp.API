using KresApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KresApp.Application.Interfaces;

public interface IMedicationRepository
{
    Task<List<Medication>> GetByChildIdAsync(Guid childId);
    Task<Medication?> GetByIdAsync(Guid id);
    Task AddAsync(Medication medication);
    Task UpdateAsync(Medication medication);
    Task DeleteAsync(Medication medication);

    Task<List<MedicationLog>> GetLogsAsync(Guid childId, DateTime date);
    Task<MedicationLog?> GetLogAsync(Guid medicationId, DateTime date, string time);
    Task AddLogAsync(MedicationLog log);
    Task UpdateLogAsync(MedicationLog log);
}
