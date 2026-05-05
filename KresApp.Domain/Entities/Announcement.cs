using KresApp.Domain.Enums;

namespace KresApp.Domain.Entities;

public class Announcement
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Title { get; private set; } = null!;
    public string Body { get; private set; } = null!;
    public AnnouncementCategory Category { get; private set; }
    public string Emoji { get; private set; } = null!;
    public DateTime Date { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Announcement() { }

    public Announcement(string title, string body, AnnouncementCategory category, string emoji, DateTime date)
    {
        Title = title;
        Body = body;
        Category = category;
        Emoji = emoji;
        Date = date;
    }

    public void Update(string title, string body, AnnouncementCategory category, string emoji)
    {
        Title = title;
        Body = body;
        Category = category;
        Emoji = emoji;
    }
}
