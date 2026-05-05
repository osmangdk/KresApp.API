using KresApp.Domain.Entities;

namespace KresApp.Application.Interfaces;

public interface IMessageRepository
{
    Task<List<Message>> GetByConversationIdAsync(Guid conversationId, int page = 1, int pageSize = 50);
    Task AddAsync(Message message);
    Task MarkAsReadAsync(Guid conversationId, Guid userId);
}
