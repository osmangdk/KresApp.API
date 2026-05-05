namespace KresApp.Domain.Entities;

public class Conversation
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string? Title { get; private set; } // Boşsa birebir konuşmadır, değilse grup
    public bool IsGroup { get; private set; }
    public Guid[] ParticipantIds { get; private set; } = Array.Empty<Guid>();
    public string? LastMessage { get; private set; }
    public DateTime? LastMessageTime { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Conversation() { }

    public Conversation(string? title, bool isGroup, Guid[] participantIds)
    {
        Title = title;
        IsGroup = isGroup;
        ParticipantIds = participantIds;
    }

    public void UpdateLastMessage(string content, DateTime time)
    {
        LastMessage = content;
        LastMessageTime = time;
    }
}
