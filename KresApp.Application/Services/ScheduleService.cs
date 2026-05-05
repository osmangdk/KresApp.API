using KresApp.Application.DTOs;
using KresApp.Application.Interfaces;
using KresApp.Domain.Entities;

namespace KresApp.Application.Services;

public class ScheduleService
{
    private readonly IScheduleRepository _repo;
    public ScheduleService(IScheduleRepository repo) => _repo = repo;

    public async Task<List<ScheduleDto>> GetAllAsync()
    {
        var items = await _repo.GetAllAsync();
        return items.Select(x => new ScheduleDto
        {
            Id = x.Id, Day = x.Day, Subject = x.Subject, 
            StartTime = x.StartTime, EndTime = x.EndTime, Teacher = x.Teacher
        }).ToList();
    }

    public async Task CreateAsync(CreateScheduleDto dto)
    {
        await _repo.AddAsync(new Schedule(dto.Day, dto.Subject, dto.StartTime, dto.EndTime, dto.Teacher));
    }

    public async Task UpdateAsync(Guid id, CreateScheduleDto dto)
    {
        var s = await _repo.GetByIdAsync(id);
        if (s == null) throw new Exception("Schedule not found");
        s.Update(dto.Day, dto.Subject, dto.StartTime, dto.EndTime, dto.Teacher);
        await _repo.UpdateAsync(s);
    }

    public async Task DeleteAsync(Guid id)
    {
        var s = await _repo.GetByIdAsync(id);
        if (s != null) await _repo.DeleteAsync(s);
    }
}
