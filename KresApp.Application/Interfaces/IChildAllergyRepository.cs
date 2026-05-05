using KresApp.Domain.Entities;

namespace KresApp.Application.Interfaces;

public interface IChildAllergyRepository
{
    Task<List<ChildAllergy>> GetByChildIdAsync(Guid childId);
    Task<ChildAllergy?> GetByIdAsync(Guid id);
    Task AddAsync(ChildAllergy allergy);
    Task UpdateAsync(ChildAllergy allergy);
    Task DeleteAsync(ChildAllergy allergy);
    Task DeleteAllByChildIdAsync(Guid childId);
}
