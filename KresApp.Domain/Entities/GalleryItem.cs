using System;

namespace KresApp.Domain.Entities;

public enum MediaType
{
    Photo = 0,
    Video = 1
}

public class GalleryItem
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string Url { get; set; } = null!;
    public string? ThumbnailUrl { get; set; }
    public MediaType Type { get; set; }
    public Guid? ClassId { get; set; }
    public virtual Class? Class { get; set; }
    public Guid? ChildId { get; set; }
    public virtual Child? Child { get; set; }
    public Guid CreatedByUserId { get; set; }
    public virtual User CreatedBy { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}
