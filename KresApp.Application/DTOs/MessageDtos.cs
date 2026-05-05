namespace KresApp.Application.DTOs;

public class ConversationDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public bool IsGroup { get; set; }
    public string? LastMessage { get; set; }
    public DateTime? LastMessageTime { get; set; }
    public int UnreadCount { get; set; }
    public Guid[] ParticipantIds { get; set; } = Array.Empty<Guid>();
}

public class MessageDto
{
    public Guid Id { get; set; }
    public Guid SenderId { get; set; }
    public string SenderName { get; set; } = null!;
    public string Content { get; set; } = null!;
    public DateTime Timestamp { get; set; }
    public bool IsRead { get; set; }
}

public class CreateMessageDto
{
    public string Content { get; set; } = null!;
}

public class CreateConversationDto
{
    public string? Title { get; set; }
    public bool IsGroup { get; set; }
    public Guid[] ParticipantIds { get; set; } = Array.Empty<Guid>();
}
