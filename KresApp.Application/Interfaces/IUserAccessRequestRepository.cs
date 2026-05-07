using KresApp.Domain.Entities;

namespace KresApp.Application.Interfaces;

public interface IUserAccessRequestRepository
{
    Task<List<UserAccessRequest>> GetAllPendingAsync();
    Task<UserAccessRequest?> GetByIdAsync(Guid id);
    Task<UserAccessRequest?> GetByEmailAsync(string email);
    Task AddAsync(UserAccessRequest request);
    Task UpdateAsync(UserAccessRequest request);
}
