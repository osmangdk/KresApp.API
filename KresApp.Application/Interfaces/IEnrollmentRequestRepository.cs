using KresApp.Domain.Entities;

namespace KresApp.Application.Interfaces;

public interface IEnrollmentRequestRepository
{
    Task<List<EnrollmentRequest>> GetAllAsync();
    Task<List<EnrollmentRequest>> GetPendingAsync();
    Task<EnrollmentRequest?> GetByIdAsync(Guid id);
    Task<EnrollmentRequest?> GetByEmailAsync(string email);
    Task AddAsync(EnrollmentRequest request);
    Task UpdateAsync(EnrollmentRequest request);
}
