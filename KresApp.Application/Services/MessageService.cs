using KresApp.Application.DTOs;
using KresApp.Application.Interfaces;
using KresApp.Domain.Entities;

namespace KresApp.Application.Services;

public class MessageService
{
    private readonly IConversationRepository _convRepo;
    private readonly IMessageRepository _msgRepo;
    private readonly IUserRepository _userRepo;

    public MessageService(IConversationRepository convRepo, IMessageRepository msgRepo, IUserRepository userRepo)
    {
        _convRepo = convRepo;
        _msgRepo = msgRepo;
        _userRepo = userRepo;
    }

    public async Task<List<ConversationDto>> GetConversationsAsync(Guid userId)
    {
        var convs = await _convRepo.GetByUserIdAsync(userId);
        return convs.Select(x => new ConversationDto
        {
            Id = x.Id, Title = x.Title ?? "", IsGroup = x.IsGroup,
            LastMessage = x.LastMessage, LastMessageTime = x.LastMessageTime,
            ParticipantIds = x.ParticipantIds, UnreadCount = 0
        }).ToList();
    }

    public async Task<List<MessageDto>> GetMessagesAsync(Guid conversationId, int page = 1)
    {
        var msgs = await _msgRepo.GetByConversationIdAsync(conversationId, page);
        return msgs.Select(x => new MessageDto
        {
            Id = x.Id, SenderId = x.SenderId, SenderName = x.SenderName,
            Content = x.Content, Timestamp = x.CreatedAt, IsRead = x.IsRead
        }).ToList();
    }

    public async Task CreateConversationAsync(Guid userId, CreateConversationDto dto)
    {
        var list = new List<Guid> { userId };
        list.AddRange(dto.ParticipantIds);
        
        var entity = new Conversation(dto.Title, dto.IsGroup, list.Distinct().ToArray());
        await _convRepo.AddAsync(entity);
    }

    public async Task SendMessageAsync(Guid conversationId, Guid senderId, CreateMessageDto dto)
    {
        var conv = await _convRepo.GetByIdAsync(conversationId);
        if (conv == null) throw new Exception("Conversation not found");

        var sender = await _userRepo.GetByIdAsync(senderId);
        
        var msg = new Message(conversationId, senderId, sender?.Name ?? "User", dto.Content, DateTime.UtcNow);
        await _msgRepo.AddAsync(msg);

        conv.UpdateLastMessage(dto.Content, msg.CreatedAt);
        await _convRepo.UpdateAsync(conv);
    }

    public async Task MarkAsReadAsync(Guid conversationId, Guid userId)
    {
        await _msgRepo.MarkAsReadAsync(conversationId, userId);
    }
}
