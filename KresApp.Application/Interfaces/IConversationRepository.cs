using KresApp.Domain.Entities;

namespace KresApp.Application.Interfaces;

public interface IConversationRepository
{
    Task<List<Conversation>> GetByUserIdAsync(Guid userId);
    Task<Conversation?> GetByIdAsync(Guid id);
    Task AddAsync(Conversation conversation);
    Task UpdateAsync(Conversation conversation);
}
