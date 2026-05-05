using KresApp.Application.DTOs;
using KresApp.Application.Interfaces;
using KresApp.Domain.Entities;

namespace KresApp.Application.Services;

public class ChildService
{
    private readonly IChildRepository _repo;
    private readonly IChildAllergyRepository _allergyRepo;
    private readonly IClassRepository _classRepo;

    public ChildService(IChildRepository repo, IChildAllergyRepository allergyRepo, IClassRepository classRepo)
    {
        _repo = repo;
        _allergyRepo = allergyRepo;
        _classRepo = classRepo;
    }

    public async Task<List<ChildDto>> Get(Guid userId, string role)
    {
        var children = await _repo.GetByFilter(userId, role);
        var result = new List<ChildDto>();
        foreach (var child in children)
        {
            var allergies = await _allergyRepo.GetByChildIdAsync(child.Id);
            result.Add(await MapToDtoAsync(child, allergies));
        }
        return result;
    }

    public async Task<Guid> Create(CreateChildDto dto)
    {
        var child = new Child(dto.Name, dto.ParentId, dto.ClassId, dto.BirthDate);
        child.UpdateProfile(dto.Name, dto.BirthDate, dto.BloodType, dto.ClassId,
                            dto.ParentName, dto.ParentPhone, dto.SecondaryPhone);
        await _repo.AddAsync(child);
        return child.Id;
    }

    public async Task<ChildDto?> GetByIdAsync(Guid id)
    {
        var child = await _repo.GetByIdAsync(id);
        if (child == null) return null;

        var allergies = await _allergyRepo.GetByChildIdAsync(id);
        return await MapToDtoAsync(child, allergies);
    }

    public async Task UpdateProfileAsync(Guid id, UpdateChildDto dto)
    {
        var child = await _repo.GetByIdAsync(id);
        if (child == null) throw new Exception("Child not found");

        child.UpdateProfile(dto.Name, dto.BirthDate, dto.BloodType, dto.ClassId, dto.ParentName, dto.ParentPhone, dto.SecondaryPhone);
        await _repo.UpdateAsync(child);
    }

    public async Task UpdateAllergiesAsync(Guid childId, UpdateAllergiesDto dto)
    {
        var child = await _repo.GetByIdAsync(childId);
        if (child == null) throw new Exception("Child not found");

        // Medikal notları güncelle
        child.UpdateMedicalNotes(dto.MedicalNotes);
        await _repo.UpdateAsync(child);

        // Mevcut alerjileri sil ve yeniden oluştur
        await _allergyRepo.DeleteAllByChildIdAsync(childId);

        foreach (var allergyDto in dto.Allergies)
        {
            var allergy = new ChildAllergy(childId, allergyDto.AllergyName, allergyDto.Severity, allergyDto.Notes);
            await _allergyRepo.AddAsync(allergy);
        }
    }

    // Tek alerji ekleme
    public async Task<ChildAllergyResponseDto> AddAllergyAsync(Guid childId, ChildAllergyDto dto)
    {
        var child = await _repo.GetByIdAsync(childId);
        if (child == null) throw new Exception("Child not found");

        var allergy = new ChildAllergy(childId, dto.AllergyName, dto.Severity, dto.Notes);
        await _allergyRepo.AddAsync(allergy);

        return MapAllergyToDto(allergy);
    }

    // Tek alerji silme
    public async Task DeleteAllergyAsync(Guid allergyId)
    {
        var allergy = await _allergyRepo.GetByIdAsync(allergyId);
        if (allergy == null) throw new Exception("Allergy not found");

        await _allergyRepo.DeleteAsync(allergy);
    }

    // Çocuğun alerjilerini getir
    public async Task<List<ChildAllergyResponseDto>> GetAllergiesAsync(Guid childId)
    {
        var allergies = await _allergyRepo.GetByChildIdAsync(childId);
        return allergies.Select(MapAllergyToDto).ToList();
    }

    public async Task DeleteAsync(Guid id)
    {
        var child = await _repo.GetByIdAsync(id);
        if (child != null)
        {
            await _allergyRepo.DeleteAllByChildIdAsync(id);
            await _repo.DeleteAsync(child);
        }
    }

    public async Task<List<ChildDto>> SearchAsync(string? q, Guid? classId)
    {
        var children = await _repo.SearchAsync(q, classId);
        var result = new List<ChildDto>();
        foreach (var child in children)
        {
            var allergies = await _allergyRepo.GetByChildIdAsync(child.Id);
            result.Add(await MapToDtoAsync(child, allergies));
        }
        return result;
    }

    private async Task<ChildDto> MapToDtoAsync(Child child, List<ChildAllergy> allergies)
    {
        string className = "Sınıfsız";
        if (child.ClassId != Guid.Empty)
        {
            var classEntity = await _classRepo.GetByIdAsync(child.ClassId);
            if (classEntity != null) className = classEntity.Name;
        }

        return new ChildDto
        {
            Id = child.Id,
            Name = child.Name,
            BirthDate = child.BirthDate,
            ClassId = child.ClassId,
            ClassName = className,
            BloodType = child.BloodType,
            Allergies = allergies.Select(MapAllergyToDto).ToList(),
            MedicalNotes = child.MedicalNotes,
            ParentName = child.ParentName,
            ParentPhone = child.ParentPhone,
            SecondaryPhone = child.SecondaryPhone,
            EnrollmentDate = child.EnrollmentDate
        };
    }

    private ChildAllergyResponseDto MapAllergyToDto(ChildAllergy allergy)
    {
        return new ChildAllergyResponseDto
        {
            Id = allergy.Id,
            AllergyName = allergy.AllergyName,
            Severity = allergy.Severity,
            Notes = allergy.Notes,
            CreatedAt = allergy.CreatedAt
        };
    }
}