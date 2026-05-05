using Microsoft.EntityFrameworkCore;
using KresApp.Application.Interfaces;
using KresApp.Domain.Entities;
using KresApp.Persistence.Context;

namespace KresApp.Persistence.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly AppDbContext _db;
    public MessageRepository(AppDbContext db) => _db = db;

    public async Task<List<Message>> GetByConversationIdAsync(Guid conversationId, int page = 1, int pageSize = 50)
    {
        return await _db.Messages
            .Where(x => x.ConversationId == conversationId)
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task AddAsync(Message message)
    {
        await _db.Messages.AddAsync(message);
        await _db.SaveChangesAsync();
    }

    public async Task MarkAsReadAsync(Guid conversationId, Guid userId)
    {
        var unreadMessages = await _db.Messages
            .Where(x => x.ConversationId == conversationId && x.SenderId != userId && !x.IsRead)
            .ToListAsync();

        foreach (var msg in unreadMessages)
        {
            msg.MarkAsRead();
        }

        if (unreadMessages.Any())
        {
            await _db.SaveChangesAsync();
        }
    }
}
