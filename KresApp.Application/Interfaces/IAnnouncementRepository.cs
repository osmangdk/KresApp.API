using KresApp.Domain.Entities;
using KresApp.Domain.Enums;

namespace KresApp.Application.Interfaces;

public interface IAnnouncementRepository
{
    Task<List<Announcement>> GetAllAsync(AnnouncementCategory? category = null);
    Task<Announcement?> GetByIdAsync(Guid id);
    Task AddAsync(Announcement announcement);
    Task UpdateAsync(Announcement announcement);
    Task DeleteAsync(Announcement announcement);
}
