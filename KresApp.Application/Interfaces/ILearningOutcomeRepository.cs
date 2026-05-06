using KresApp.Domain.Entities;

namespace KresApp.Application.Interfaces;

public interface ILearningOutcomeRepository
{
    Task<List<LearningOutcome>> GetAllAsync(int? year = null);
    Task<LearningOutcome?> GetByMonthYearAsync(int month, int year);
    Task<LearningOutcome?> GetByIdAsync(Guid id);
    Task AddAsync(LearningOutcome outcome);
    Task UpdateAsync(LearningOutcome outcome);
    Task DeleteAsync(LearningOutcome outcome);
}
