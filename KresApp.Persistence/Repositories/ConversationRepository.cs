using Microsoft.EntityFrameworkCore;
using KresApp.Application.Interfaces;
using KresApp.Domain.Entities;
using KresApp.Persistence.Context;

namespace KresApp.Persistence.Repositories;

public class ConversationRepository : IConversationRepository
{
    private readonly AppDbContext _db;
    public ConversationRepository(AppDbContext db) => _db = db;

    public async Task<List<Conversation>> GetByUserIdAsync(Guid userId)
    {
        return await _db.Conversations
            .Where(x => x.ParticipantIds.Contains(userId))
            .OrderByDescending(x => x.LastMessageTime)
            .ToListAsync();
    }

    public async Task<Conversation?> GetByIdAsync(Guid id)
    {
        return await _db.Conversations.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task AddAsync(Conversation conversation)
    {
        await _db.Conversations.AddAsync(conversation);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Conversation conversation)
    {
        _db.Conversations.Update(conversation);
        await _db.SaveChangesAsync();
    }
}
