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
    private readonly IConversationRepository _convRepo;
    private readonly IMessageRepository _msgRepo;
    private readonly IUserRepository _userRepo;
    private readonly IChildRepository _childRepo;

    public MeetingRequestService(
        IMeetingRequestRepository repo,
        IConversationRepository convRepo,
        IMessageRepository msgRepo,
        IUserRepository userRepo,
        IChildRepository childRepo)
    {
        _repo = repo;
        _convRepo = convRepo;
        _msgRepo = msgRepo;
        _userRepo = userRepo;
        _childRepo = childRepo;
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

        // Fetch child and parent name for the notification message
        var child = await _childRepo.GetByIdAsync(dto.ChildId);
        var childName = child?.Name ?? "Öğrenci";

        var parent = await _userRepo.GetByIdAsync(parentId);
        var parentName = parent?.Name ?? "Veli";

        string formattedDate = dto.PlannedDate.ToString("dd.MM.yyyy HH:mm");
        string msgContent = $"🤝 *YENİ GÖRÜŞME TALEBİ*\n\nMerhaba, veli {parentName}, {childName} isimli öğrencimiz için {formattedDate} tarihinde sizinle bir görüşme talep etti.\n\n*Görüşme Nedeni:*\n\"{dto.RequestNote}\"\n\nLütfen 'Görüşme Talepleri' ekranından onaylayın veya güncelleyin.";

        await SendSystemMessageAsync(parentId, dto.TeacherId, parentId, msgContent);
    }

    public async Task ApproveAsync(Guid id, ApproveMeetingRequestDto dto, Guid approvedByUserId)
    {
        var request = await _repo.GetByIdAsync(id);
        if (request == null) throw new Exception("Görüşme talebi bulunamadı.");

        request.Approve(dto.PlannedDate, dto.AdminNote);
        await _repo.UpdateAsync(request);

        string formattedDate = dto.PlannedDate.ToString("dd.MM.yyyy HH:mm");
        string msgContent = $"✅ *GÖRÜŞME TALEBİ ONAYLANDI*\n\nGörüşme talebiniz onaylandı ve planlandı.\n\n*Tarih/Saat:* {formattedDate}\n{(string.IsNullOrEmpty(dto.AdminNote) ? "" : $"*Okul Notu/Açıklama:* {dto.AdminNote}")}";

        await SendSystemMessageAsync(request.ParentId, request.TeacherId, approvedByUserId, msgContent);
    }

    public async Task RejectAsync(Guid id, RejectMeetingRequestDto dto, Guid rejectedByUserId)
    {
        var request = await _repo.GetByIdAsync(id);
        if (request == null) throw new Exception("Görüşme talebi bulunamadı.");

        request.Reject(dto.AdminNote);
        await _repo.UpdateAsync(request);

        string msgContent = $"❌ *GÖRÜŞME TALEBİ REDDEDİLDİ*\n\nGörüşme talebiniz maalesef uygun bulunamadı.\n\n*Gerekçe:* {dto.AdminNote}";

        await SendSystemMessageAsync(request.ParentId, request.TeacherId, rejectedByUserId, msgContent);
    }

    public async Task CancelAsync(Guid id, string? note, Guid cancelledByUserId)
    {
        var request = await _repo.GetByIdAsync(id);
        if (request == null) throw new Exception("Görüşme talebi bulunamadı.");

        request.Cancel(note);
        await _repo.UpdateAsync(request);

        string msgContent = $"⚠️ *GÖRÜŞME TALEBİ İPTAL EDİLDİ*\n\nGörüşme talebi iptal edildi.\n\n*Not:* {note ?? "İptal edildi."}";

        await SendSystemMessageAsync(request.ParentId, request.TeacherId, cancelledByUserId, msgContent);
    }

    public async Task CompleteAsync(Guid id, CompleteMeetingRequestDto dto, Guid completedByUserId)
    {
        var request = await _repo.GetByIdAsync(id);
        if (request == null) throw new Exception("Görüşme talebi bulunamadı.");
        if (request.Status != MeetingStatus.Approved) throw new Exception("Sadece onaylanmış görüşmeler tamamlanabilir.");

        request.Complete(dto.MeetingNotes, dto.AttachmentUrl);
        await _repo.UpdateAsync(request);

        string msgContent = $"📋 *GÖRÜŞME TAMAMLANDI VE RAPOR EKLENDİ*\n\nGörüşme tamamlanmıştır. Görüşme notlarını ve raporu 'Görüşme Talepleri' sayfasından inceleyebilirsiniz.\n\n*Özet Rapor Notu:*\n\"{dto.MeetingNotes}\"";

        await SendSystemMessageAsync(request.ParentId, request.TeacherId, completedByUserId, msgContent);
    }

    private async Task SendSystemMessageAsync(Guid parentId, Guid teacherId, Guid senderId, string content)
    {
        try
        {
            var conversations = await _convRepo.GetByUserIdAsync(parentId);
            var conv = conversations.FirstOrDefault(c => !c.IsGroup &&
                c.ParticipantIds.Contains(teacherId));

            if (conv == null)
            {
                conv = new Conversation(null, false, new Guid[] { parentId, teacherId });
                await _convRepo.AddAsync(conv);
            }

            var sender = await _userRepo.GetByIdAsync(senderId);
            var senderName = sender?.Name ?? "Sistem";

            var msg = new Message(conv.Id, senderId, senderName, content, DateTime.UtcNow);
            await _msgRepo.AddAsync(msg);

            conv.UpdateLastMessage(content, msg.CreatedAt);
            await _convRepo.UpdateAsync(conv);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Notification Error] Failed to send meeting notification message: {ex.Message}");
        }
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
