using KresApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KresApp.Application.Interfaces;

public interface IChildHealthRepository
{
    Task<List<ChildHealthRecord>> GetByChildIdAsync(Guid childId);
    Task<ChildHealthRecord?> GetByIdAsync(Guid id);
    Task AddAsync(ChildHealthRecord record);
    Task UpdateAsync(ChildHealthRecord record);
    Task DeleteAsync(ChildHealthRecord record);
    Task AddRangeAsync(IEnumerable<ChildHealthRecord> records);
}
