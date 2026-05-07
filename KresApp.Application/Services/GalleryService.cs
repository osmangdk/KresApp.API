using KresApp.Application.DTOs;
using KresApp.Application.Interfaces;
using KresApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KresApp.Application.Services;

public class GalleryService
{
    private readonly IGalleryRepository _repo;
    private readonly IFileStorageService _storage;

    public GalleryService(IGalleryRepository repo, IFileStorageService storage)
    {
        _repo = repo;
        _storage = storage;
    }

    public async Task<List<GalleryItemDto>> GetForUserAsync(Guid? classId, Guid? childId)
    {
        List<GalleryItem> items;
        if (childId.HasValue)
        {
            // If parent, get items for their child OR their class
            items = await _repo.GetByChildIdAsync(childId.Value);
            if (classId.HasValue)
            {
                var classItems = await _repo.GetByClassIdAsync(classId.Value);
                items.AddRange(classItems);
            }
        }
        else if (classId.HasValue)
        {
            items = await _repo.GetByClassIdAsync(classId.Value);
        }
        else
        {
            items = await _repo.GetAllAsync();
        }

        return items.DistinctBy(x => x.Id).OrderByDescending(x => x.CreatedAt).Select(MapToDto).ToList();
    }

    public async Task CreateAsync(CreateGalleryItemDto dto, Guid userId)
    {
        string? url = dto.Url;

        if (dto.File != null)
        {
            using var stream = dto.File.OpenReadStream();
            url = await _storage.UploadFileAsync(stream, dto.File.FileName, dto.File.ContentType);
        }

        if (string.IsNullOrEmpty(url))
        {
            throw new Exception("Medya dosyası veya URL gereklidir.");
        }

        var item = new GalleryItem
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            Description = dto.Description,
            Url = url,
            ThumbnailUrl = dto.ThumbnailUrl,
            Type = dto.Type,
            ClassId = dto.ClassId,
            ChildId = dto.ChildId,
            CreatedByUserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(item);
    }

    public async Task DeleteAsync(Guid id)
    {
        var item = await _repo.GetByIdAsync(id);
        if (item != null) await _repo.DeleteAsync(item);
    }

    private GalleryItemDto MapToDto(GalleryItem x) => new()
    {
        Id = x.Id,
        Title = x.Title,
        Description = x.Description,
        Url = x.Url,
        ThumbnailUrl = x.ThumbnailUrl,
        Type = x.Type,
        ClassId = x.ClassId,
        ChildId = x.ChildId,
        CreatedByName = x.CreatedBy?.Name ?? "Bilinmiyor",
        CreatedAt = x.CreatedAt
    };
}
