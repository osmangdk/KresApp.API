using KresApp.Application.DTOs;
using KresApp.Application.Interfaces;
using KresApp.Domain.Entities;

namespace KresApp.Application.Services;

public class DailyReportService
{
    private readonly IDailyReportRepository _repo;
    private readonly IChildRepository _childRepo;

    public DailyReportService(IDailyReportRepository repo, IChildRepository childRepo)
    {
        _repo = repo;
        _childRepo = childRepo;
    }

    public async Task<List<DailyReportDto>> GetByDateAsync(DateOnly date, Guid? classId = null)
    {
        var items = await _repo.GetByDateAsync(date, classId);
        var result = new List<DailyReportDto>();

        foreach (var x in items)
        {
            var child = await _childRepo.GetByIdAsync(x.ChildId);
            result.Add(MapToDto(x, child?.Name ?? "Bilinmiyor"));
        }
        return result;
    }

    public async Task<DailyReportDto?> GetForChildAsync(Guid childId, DateOnly date)
    {
        var x = await _repo.GetByChildAndDateAsync(childId, date);
        if (x == null) return null;
        
        var child = await _childRepo.GetByIdAsync(childId);
        return MapToDto(x, child?.Name ?? "");
    }

    public async Task CreateAsync(CreateDailyReportDto dto)
    {
        var existing = await _repo.GetByChildAndDateAsync(dto.ChildId, dto.Date);
        if (existing != null) throw new Exception("Report already exists for this date");

        var report = new DailyReport(dto.ChildId, dto.Date, dto.Mood, dto.MorningMeal, dto.LunchMeal, dto.AfternoonMeal,
            dto.DidSleep, dto.SleepHours, dto.SleepMins, dto.ToiletCount, dto.Activities, dto.TeacherNote);
            
        await _repo.AddAsync(report);
    }

    public async Task UpdateAsync(Guid id, CreateDailyReportDto dto)
    {
        var report = await _repo.GetByIdAsync(id);
        if (report == null) throw new Exception("Report not found");

        report.Update(dto.Mood, dto.MorningMeal, dto.LunchMeal, dto.AfternoonMeal,
            dto.DidSleep, dto.SleepHours, dto.SleepMins, dto.ToiletCount, dto.Activities, dto.TeacherNote);
            
        await _repo.UpdateAsync(report);
    }

    private DailyReportDto MapToDto(DailyReport x, string childName)
    {
        return new DailyReportDto
        {
            Id = x.Id, ChildId = x.ChildId, ChildName = childName, Date = x.Date,
            Mood = x.Mood, MorningMeal = x.MorningMeal, LunchMeal = x.LunchMeal, AfternoonMeal = x.AfternoonMeal,
            DidSleep = x.DidSleep, SleepHours = x.SleepHours, SleepMins = x.SleepMins, ToiletCount = x.ToiletCount,
            Activities = x.Activities, TeacherNote = x.TeacherNote
        };
    }
}
