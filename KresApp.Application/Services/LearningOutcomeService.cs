using KresApp.Application.DTOs;
using KresApp.Application.Interfaces;
using KresApp.Domain.Entities;

namespace KresApp.Application.Services;

public class LearningOutcomeService
{
    private readonly ILearningOutcomeRepository _repo;

    private static readonly string[] TurkishMonths =
    {
        "Ocak", "Şubat", "Mart", "Nisan", "Mayıs", "Haziran",
        "Temmuz", "Ağustos", "Eylül", "Ekim", "Kasım", "Aralık"
    };

    public LearningOutcomeService(ILearningOutcomeRepository repo) => _repo = repo;

    public async Task<List<LearningOutcomeDto>> GetAllAsync(int? year = null)
    {
        var items = await _repo.GetAllAsync(year);
        return items.Select(MapToDto).OrderBy(x => x.Year).ThenBy(x => x.Month).ToList();
    }

    public async Task<LearningOutcomeDto?> GetByIdAsync(Guid id)
    {
        var item = await _repo.GetByIdAsync(id);
        return item == null ? null : MapToDto(item);
    }

    public async Task<LearningOutcomeDto> CreateAsync(CreateLearningOutcomeDto dto)
    {
        var entity = new LearningOutcome
        {
            Id = Guid.NewGuid(),
            Month = dto.Month,
            Year = dto.Year,
            Theme = dto.Theme,
            AgeGroupId = dto.AgeGroupId,
            ClassId = dto.ClassId,
            Outcomes = dto.Outcomes,
            Description = dto.Description,
            CreatedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(entity);
        return MapToDto(entity);
    }

    public async Task UpdateAsync(Guid id, CreateLearningOutcomeDto dto)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity == null) throw new Exception("Kazanım kaydı bulunamadı.");

        entity.Month = dto.Month;
        entity.Year = dto.Year;
        entity.Theme = dto.Theme;
        entity.AgeGroupId = dto.AgeGroupId;
        entity.ClassId = dto.ClassId;
        entity.Outcomes = dto.Outcomes;
        entity.Description = dto.Description;

        await _repo.UpdateAsync(entity);
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity != null) await _repo.DeleteAsync(entity);
    }

    private LearningOutcomeDto MapToDto(LearningOutcome entity) => new()
    {
        Id = entity.Id,
        Month = entity.Month,
        Year = entity.Year,
        MonthName = TurkishMonths[entity.Month - 1],
        Theme = entity.Theme,
        AgeGroupId = entity.AgeGroupId,
        AgeGroupName = entity.AgeGroup?.Name,
        ClassId = entity.ClassId,
        ClassName = entity.Class?.Name,
        Outcomes = entity.Outcomes,
        Description = entity.Description,
        CreatedAt = entity.CreatedAt
    };
}
