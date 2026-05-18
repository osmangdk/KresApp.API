using KresApp.Application.DTOs;
using KresApp.Application.Interfaces;
using KresApp.Domain.Entities;
using KresApp.Domain.Enums;

namespace KresApp.Application.Services;

public class LeaveRequestService
{
    private readonly ILeaveRequestRepository _repo;
    private readonly IAttendanceRepository _attendanceRepo;

    public LeaveRequestService(ILeaveRequestRepository repo, IAttendanceRepository attendanceRepo)
    {
        _repo = repo;
        _attendanceRepo = attendanceRepo;
    }

    public async Task<List<LeaveRequestDto>> GetByChildAsync(Guid childId)
    {
        var items = await _repo.GetByChildIdAsync(childId);
        return items.Select(x => MapToDto(x)).ToList();
    }

    public async Task<List<LeaveRequestDto>> GetPendingByClassAsync(Guid classId)
    {
        var items = await _repo.GetPendingByClassIdAsync(classId);
        return items.Select(x => MapToDto(x)).ToList();
    }

    public async Task CreateAsync(CreateLeaveRequestDto dto)
    {
        var request = new LeaveRequest(dto.ChildId, dto.StartDate, dto.EndDate, dto.Reason);
        await _repo.AddAsync(request);
    }

    public async Task ApproveAsync(Guid id, Guid approvedByUserId, string? adminNote = null)
    {
        var request = await _repo.GetByIdAsync(id);
        if (request == null) throw new Exception("İzin talebi bulunamadı.");

        request.Approve(approvedByUserId, adminNote);
        await _repo.UpdateAsync(request);

        // Yoklamayı otomatik "İzinli" olarak güncelle
        await UpdateAttendanceAsExcused(request);
    }

    public async Task RejectAsync(Guid id, Guid approvedByUserId, string? adminNote = null)
    {
        var request = await _repo.GetByIdAsync(id);
        if (request == null) throw new Exception("İzin talebi bulunamadı.");

        request.Reject(approvedByUserId, adminNote);
        await _repo.UpdateAsync(request);
    }

    private async Task UpdateAttendanceAsExcused(LeaveRequest request)
    {
        var startDate = DateOnly.FromDateTime(request.StartDate);
        var endDate = DateOnly.FromDateTime(request.EndDate);

        // Mevcut yoklama kayıtlarını çek
        var existingAttendances = await _attendanceRepo.GetByChildAsync(request.ChildId, startDate, endDate);

        var toAdd = new List<Attendance>();
        
        for (var date = startDate; date <= endDate; date = date.AddDays(1))
        {
            // Hafta sonu ise atla (Opsiyonel: Kreşiniz hafta sonu açık mı?)
            if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday) continue;

            var existing = existingAttendances.FirstOrDefault(x => x.Date == date);
            if (existing != null)
            {
                existing.UpdateStatus(AttendanceStatus.Excused);
                await _attendanceRepo.UpdateAsync(existing);
            }
            else
            {
                toAdd.Add(new Attendance(request.ChildId, date, AttendanceStatus.Excused));
            }
        }

        if (toAdd.Any())
        {
            await _attendanceRepo.AddRangeAsync(toAdd);
        }
    }

    private LeaveRequestDto MapToDto(LeaveRequest x)
    {
        return new LeaveRequestDto
        {
            Id = x.Id,
            ChildId = x.ChildId,
            ChildFullName = x.Child?.Name ?? "",
            StartDate = x.StartDate,
            EndDate = x.EndDate,
            Reason = x.Reason,
            Status = x.Status,
            CreatedAt = x.CreatedAt,
            AdminNote = x.AdminNote
        };
    }
}
