using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KresApp.Application.DTOs;
using KresApp.Application.Interfaces;
using KresApp.Domain.Entities;
using KresApp.Domain.Enums;

namespace KresApp.Application.Services;

public class MeetingRequestService
{
    private readonly IMeetingRequestRepository _repo;

    public MeetingRequestService(IMeetingRequestRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<MeetingRequestDto>> GetAllAsync()
    {
        var items = await _repo.GetAllAsync();
        return items.Select(MapToDto).ToList();
    }

    public async Task<List<MeetingRequestDto>> GetByParentAsync(Guid parentId)
    {
        var items = await _repo.GetByParentIdAsync(parentId);
        return items.Select(MapToDto).ToList();
    }

    public async Task<List<MeetingRequestDto>> GetByTeacherAsync(Guid teacherId)
    {
        var items = await _repo.GetByTeacherIdAsync(teacherId);
        return items.Select(MapToDto).ToList();
    }

    public async Task<List<MeetingRequestDto>> GetByChildAsync(Guid childId)
    {
        var items = await _repo.GetByChildIdAsync(childId);
        return items.Select(MapToDto).ToList();
    }

    public async Task CreateAsync(Guid parentId, CreateMeetingRequestDto dto)
    {
        var request = new MeetingRequest(
            dto.ChildId,
            parentId,
            dto.TeacherId,
            dto.PlannedDate,
            dto.RequestNote,
            dto.DurationMinutes
        );
        await _repo.AddAsync(request);
    }

    public async Task ApproveAsync(Guid id, ApproveMeetingRequestDto dto)
    {
        var request = await _repo.GetByIdAsync(id);
        if (request == null) throw new Exception("Görüşme talebi bulunamadı.");

        request.Approve(dto.PlannedDate, dto.AdminNote);
        await _repo.UpdateAsync(request);
    }

    public async Task RejectAsync(Guid id, RejectMeetingRequestDto dto)
    {
        var request = await _repo.GetByIdAsync(id);
        if (request == null) throw new Exception("Görüşme talebi bulunamadı.");

        request.Reject(dto.AdminNote);
        await _repo.UpdateAsync(request);
    }

    public async Task CancelAsync(Guid id, string? note = null)
    {
        var request = await _repo.GetByIdAsync(id);
        if (request == null) throw new Exception("Görüşme talebi bulunamadı.");

        request.Cancel(note);
        await _repo.UpdateAsync(request);
    }

    public async Task CompleteAsync(Guid id, CompleteMeetingRequestDto dto)
    {
        var request = await _repo.GetByIdAsync(id);
        if (request == null) throw new Exception("Görüşme talebi bulunamadı.");
        if (request.Status != MeetingStatus.Approved) throw new Exception("Sadece onaylanmış görüşmeler tamamlanabilir.");

        request.Complete(dto.MeetingNotes, dto.AttachmentUrl);
        await _repo.UpdateAsync(request);
    }

    private MeetingRequestDto MapToDto(MeetingRequest x)
    {
        return new MeetingRequestDto
        {
            Id = x.Id,
            ChildId = x.ChildId,
            ChildFullName = x.Child?.Name ?? "Bilinmiyor",
            ParentId = x.ParentId,
            ParentName = x.Parent?.Name ?? "Bilinmiyor",
            TeacherId = x.TeacherId,
            TeacherName = x.Teacher?.Name ?? "Bilinmiyor",
            PlannedDate = x.PlannedDate,
            DurationMinutes = x.DurationMinutes,
            RequestNote = x.RequestNote,
            AdminNote = x.AdminNote,
            MeetingNotes = x.MeetingNotes,
            AttachmentUrl = x.AttachmentUrl,
            Status = (int)x.Status,
            StatusText = x.Status switch
            {
                MeetingStatus.Pending => "Beklemede",
                MeetingStatus.Approved => "Onaylandı / Planlandı",
                MeetingStatus.Rejected => "Reddedildi",
                MeetingStatus.Completed => "Tamamlandı",
                MeetingStatus.Cancelled => "İptal Edildi",
                _ => "Bilinmeyen Durum"
            },
            CreatedAt = x.CreatedAt
        };
    }
}
