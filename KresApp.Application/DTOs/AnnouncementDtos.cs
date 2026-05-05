using KresApp.Domain.Enums;

namespace KresApp.Application.DTOs;

public class AnnouncementDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Body { get; set; } = null!;
    public AnnouncementCategory Category { get; set; }
    public string Emoji { get; set; } = null!;
    public DateTime Date { get; set; }
    public bool IsRead { get; set; } 
}

public class CreateAnnouncementDto
{
    public string Title { get; set; } = null!;
    public string Body { get; set; } = null!;
    public AnnouncementCategory Category { get; set; }
    public string Emoji { get; set; } = null!;
}
