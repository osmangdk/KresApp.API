using KresApp.Domain.Entities;
using System;

namespace KresApp.Application.DTOs;

public class GalleryItemDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string Url { get; set; } = null!;
    public string? ThumbnailUrl { get; set; }
    public MediaType Type { get; set; }
    public Guid? ClassId { get; set; }
    public Guid? ChildId { get; set; }
    public string CreatedByName { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}

public class CreateGalleryItemDto
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string Url { get; set; } = null!;
    public string? ThumbnailUrl { get; set; }
    public MediaType Type { get; set; }
    public Guid? ClassId { get; set; }
    public Guid? ChildId { get; set; }
}
