namespace KresApp.Domain.Entities;

public class Message
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid ConversationId { get; private set; }
    public Guid SenderId { get; private set; }
    public string SenderName { get; private set; } = null!;
    public string Content { get; private set; } = null!;
    public bool IsRead { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Message() { }

    public Message(Guid conversationId, Guid senderId, string senderName, string content, DateTime timestamp)
    {
        ConversationId = conversationId;
        SenderId = senderId;
        SenderName = senderName;
        Content = content;
        CreatedAt = timestamp;
    }

    public void MarkAsRead()
    {
        IsRead = true;
    }
}
