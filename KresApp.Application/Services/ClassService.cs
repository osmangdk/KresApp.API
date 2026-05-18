using KresApp.Application.DTOs;
using KresApp.Application.Interfaces;
using KresApp.Domain.Entities;

namespace KresApp.Application.Services;

public class ClassService
{
    private readonly IClassRepository _repo;

    public ClassService(IClassRepository repo) => _repo = repo;

    public async Task<List<ClassDto>> GetAllAsync()
    {
        var items = await _repo.GetAllAsync();
        return items.Select(x => new ClassDto
        {
            Id = x.Id,
            Name = x.Name,
            TeacherId = x.TeacherId,
            AgeGroupId = x.AgeGroupId,
            AgeGroupName = x.AgeGroup?.Name
        }).ToList();
    }

    public async Task<ClassDto?> GetByIdAsync(Guid id)
    {
        var x = await _repo.GetByIdAsync(id);
        if (x == null) return null;
        return new ClassDto
        {
            Id = x.Id,
            Name = x.Name,
            TeacherId = x.TeacherId,
            AgeGroupId = x.AgeGroupId,
            AgeGroupName = x.AgeGroup?.Name
        };
    }

    public async Task CreateAsync(CreateClassDto dto)
    {
        var all = await _repo.GetAllAsync();
        if (all.Any(c => c.TeacherId == dto.TeacherId))
            throw new Exception("Bu öğretmen zaten bir sınıfa atanmış.");

        var classEntity = new Class(dto.Name, dto.TeacherId, dto.AgeGroupId);
        await _repo.AddAsync(classEntity);
    }

    public async Task UpdateAsync(Guid id, CreateClassDto dto)
    {
        var classEntity = await _repo.GetByIdAsync(id);
        if (classEntity == null) throw new Exception("Sınıf bulunamadı.");

        var all = await _repo.GetAllAsync();
        if (all.Any(c => c.TeacherId == dto.TeacherId && c.Id != id))
            throw new Exception("Bu öğretmen zaten başka bir sınıfa atanmış.");

        classEntity.Update(dto.Name, dto.TeacherId, dto.AgeGroupId);
        await _repo.UpdateAsync(classEntity);
    }

    public async Task DeleteAsync(Guid id)
    {
        var classEntity = await _repo.GetByIdAsync(id);
        if (classEntity != null) await _repo.DeleteAsync(classEntity);
    }
}
