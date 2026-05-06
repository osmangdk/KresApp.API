using KresApp.Application.DTOs;
using KresApp.Application.Interfaces;
using KresApp.Domain.Entities;
using KresApp.Domain.Enums;

namespace KresApp.Application.Services;

public class SchoolBellService
{
    private readonly ISchoolBellRepository _repository;

    public SchoolBellService(ISchoolBellRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<SchoolBellRequestDto>> GetAllAsync(DateTime? date = null)
    {
        var requests = await _repository.GetAllAsync(date);
        return requests.Select(MapToDto).ToList();
    }

    public async Task<List<SchoolBellRequestDto>> GetByParentAsync(Guid parentId)
    {
        var requests = await _repository.GetByParentAsync(parentId);
        return requests.Select(MapToDto).ToList();
    }

    public async Task<SchoolBellRequestDto> CreateAsync(Guid parentId, CreateSchoolBellRequestDto dto)
    {
        var request = new SchoolBellRequest
        {
            Id = Guid.NewGuid(),
            ParentId = parentId,
            ChildId = dto.ChildId,
            Type = dto.Type,
            Status = SchoolBellStatus.Pending,
            Note = dto.Note,
            EstimatedTime = dto.EstimatedTime,
            CreatedAt = DateTime.UtcNow.AddHours(3) // Turkey time
        };

        await _repository.AddAsync(request);
        
        // Fetch again to get related entities for the DTO
        var created = await _repository.GetByIdAsync(request.Id);
        return MapToDto(created!);
    }

    public async Task UpdateStatusAsync(Guid id, SchoolBellStatus status)
    {
        var request = await _repository.GetByIdAsync(id);
        if (request == null) throw new Exception("Talep bulunamadı.");

        request.Status = status;
        await _repository.UpdateAsync(request);
    }

    private SchoolBellRequestDto MapToDto(SchoolBellRequest entity)
    {
        return new SchoolBellRequestDto
        {
            Id = entity.Id,
            ChildId = entity.ChildId,
            ChildName = entity.Child?.Name ?? "Bilinmiyor",
            ParentId = entity.ParentId,
            ParentName = entity.Parent?.Name ?? "Bilinmiyor",
            Type = entity.Type,
            Status = entity.Status,
            Note = entity.Note,
            EstimatedTime = entity.EstimatedTime,
            CreatedAt = entity.CreatedAt
        };
    }
}
