using KresApp.Application.DTOs;
using KresApp.Application.Interfaces;
using KresApp.Domain.Entities;
using KresApp.Domain.Enums;

namespace KresApp.Application.Services;

public class AnnouncementService
{
    private readonly IAnnouncementRepository _repo;

    public AnnouncementService(IAnnouncementRepository repo) => _repo = repo;

    public async Task<List<AnnouncementDto>> GetAllAsync(AnnouncementCategory? category)
    {
        var items = await _repo.GetAllAsync(category);
        return items.Select(x => new AnnouncementDto
        {
            Id = x.Id,
            Title = x.Title,
            Body = x.Body,
            Category = x.Category,
            Emoji = x.Emoji,
            Date = x.Date,
            IsRead = false
        }).ToList();
    }

    public async Task<AnnouncementDto?> GetByIdAsync(Guid id)
    {
        var x = await _repo.GetByIdAsync(id);
        if (x == null) return null;
        return new AnnouncementDto
        {
            Id = x.Id,
            Title = x.Title,
            Body = x.Body,
            Category = x.Category,
            Emoji = x.Emoji,
            Date = x.Date
        };
    }

    public async Task CreateAsync(CreateAnnouncementDto dto)
    {
        var ann = new Announcement(dto.Title, dto.Body, dto.Category, dto.Emoji, DateTime.UtcNow);
        await _repo.AddAsync(ann);
    }

    public async Task UpdateAsync(Guid id, CreateAnnouncementDto dto)
    {
        var ann = await _repo.GetByIdAsync(id);
        if (ann == null) throw new Exception("Announcement not found");

        ann.Update(dto.Title, dto.Body, dto.Category, dto.Emoji);
        await _repo.UpdateAsync(ann);
    }

    public async Task DeleteAsync(Guid id)
    {
        var ann = await _repo.GetByIdAsync(id);
        if (ann != null) await _repo.DeleteAsync(ann);
    }
}
