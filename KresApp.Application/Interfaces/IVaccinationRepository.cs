using KresApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KresApp.Application.Interfaces;

public interface IVaccinationRepository
{
    Task<List<Vaccination>> GetByChildIdAsync(Guid childId);
    Task<Vaccination?> GetByIdAsync(Guid id);
    Task AddAsync(Vaccination vaccination);
    Task UpdateAsync(Vaccination vaccination);
    Task DeleteAsync(Vaccination vaccination);
}
