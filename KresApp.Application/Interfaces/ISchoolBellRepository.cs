using KresApp.Domain.Entities;

namespace KresApp.Application.Interfaces;

public interface ISchoolBellRepository
{
    Task<List<SchoolBellRequest>> GetAllAsync(DateTime? date = null);
    Task<List<SchoolBellRequest>> GetByParentAsync(Guid parentId);
    Task<SchoolBellRequest?> GetByIdAsync(Guid id);
    Task AddAsync(SchoolBellRequest request);
    Task UpdateAsync(SchoolBellRequest request);
}
