using KresApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KresApp.Application.Interfaces;

public interface IGalleryRepository
{
    Task<List<GalleryItem>> GetByClassIdAsync(Guid classId);
    Task<List<GalleryItem>> GetByChildIdAsync(Guid childId);
    Task<List<GalleryItem>> GetAllAsync();
    Task<GalleryItem?> GetByIdAsync(Guid id);
    Task AddAsync(GalleryItem item);
    Task DeleteAsync(GalleryItem item);
}
