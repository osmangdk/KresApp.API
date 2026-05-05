using KresApp.Application.DTOs;
using KresApp.Application.Interfaces;
using KresApp.Domain.Entities;

namespace KresApp.Application.Services;

public class AttendanceService
{
    private readonly IAttendanceRepository _repo;
    private readonly IChildRepository _childRepo;

    public AttendanceService(IAttendanceRepository repo, IChildRepository childRepo)
    {
        _repo = repo;
        _childRepo = childRepo;
    }

    public async Task<List<AttendanceDto>> GetByDateAsync(DateOnly date, Guid? classId = null)
    {
        var records = await _repo.GetByDateAsync(date, classId);
        var result = new List<AttendanceDto>();
        foreach (var r in records)
        {
            var child = await _childRepo.GetByIdAsync(r.ChildId);
            result.Add(new AttendanceDto
            {
                Id = r.Id,
                ChildId = r.ChildId,
                ChildName = child?.Name ?? "Bilinmiyor",
                Date = r.Date,
                Status = r.Status
            });
        }
        return result;
    }

    public async Task CreateBulkAsync(CreateBulkAttendanceDto dto)
    {
        var entities = dto.Records.Select(r => new Attendance(r.ChildId, dto.Date, r.Status));
        await _repo.AddRangeAsync(entities);
    }

    public async Task<List<AttendanceDto>> GetByChildAsync(Guid childId, string? month)
    {
        var records = await _repo.GetByChildAsync(childId);
        return records.Select(r => new AttendanceDto
        {
            Id = r.Id,
            ChildId = r.ChildId,
            ChildName = "", // History has no need for repeating names
            Date = r.Date,
            Status = r.Status
        }).ToList();
    }

    public async Task<AttendanceSummaryDto> GetSummaryAsync(DateOnly date)
    {
        var records = await _repo.GetByDateAsync(date);
        var total = records.Count;
        if (total == 0) return new AttendanceSummaryDto { Date = date };

        var present = records.Count(x => x.Status == Domain.Enums.AttendanceStatus.Present);
        var absent = records.Count(x => x.Status == Domain.Enums.AttendanceStatus.Absent);
        var late = records.Count(x => x.Status == Domain.Enums.AttendanceStatus.Late);

        return new AttendanceSummaryDto
        {
            Date = date,
            Total = total,
            Present = present,
            Absent = absent,
            Late = late,
            Rate = Math.Round((double)present / total * 100, 2)
        };
    }
}
