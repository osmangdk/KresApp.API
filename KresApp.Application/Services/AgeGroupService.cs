using KresApp.Application.DTOs;
using KresApp.Application.Interfaces;
using KresApp.Domain.Entities;

namespace KresApp.Application.Services;

public class AgeGroupService
{
    private readonly IAgeGroupRepository _repo;

    public AgeGroupService(IAgeGroupRepository repo) => _repo = repo;

    public async Task<List<AgeGroupDto>> GetAllAsync()
    {
        var items = await _repo.GetAllAsync();
        return items.Select(x => new AgeGroupDto
        {
            Id = x.Id,
            Name = x.Name,
            Quota = x.Quota,
            Description = x.Description
        }).ToList();
    }

    public async Task<AgeGroupDto?> GetByIdAsync(Guid id)
    {
        var x = await _repo.GetByIdAsync(id);
        if (x == null) return null;
        return new AgeGroupDto
        {
            Id = x.Id,
            Name = x.Name,
            Quota = x.Quota,
            Description = x.Description
        };
    }

    public async Task<AgeGroupDto> CreateAsync(CreateAgeGroupDto dto)
    {
        var entity = new AgeGroup
        {
            Name = dto.Name,
            Quota = dto.Quota,
            Description = dto.Description
        };

        await _repo.AddAsync(entity);

        return new AgeGroupDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Quota = entity.Quota,
            Description = entity.Description
        };
    }

    public async Task UpdateAsync(Guid id, CreateAgeGroupDto dto)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity == null) throw new Exception("Yaş grubu bulunamadı.");

        entity.Name = dto.Name;
        entity.Quota = dto.Quota;
        entity.Description = dto.Description;

        await _repo.UpdateAsync(entity);
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity != null)
        {
            await _repo.DeleteAsync(entity);
        }
    }
}
