using System;
using KresApp.Domain.Enums;

namespace KresApp.Domain.Entities;

public class MeetingRequest
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    
    // Görüşülecek çocuk
    public Guid ChildId { get; private set; }
    public Child Child { get; private set; } = null!;
    
    // Görüşmeyi talep eden veli
    public Guid ParentId { get; private set; }
    public User Parent { get; private set; } = null!;
    
    // Görüşülecek öğretmen
    public Guid TeacherId { get; private set; }
    public User Teacher { get; private set; } = null!;
    
    // Görüşme planlanan tarih ve saat
    public DateTime PlannedDate { get; private set; }
    public int DurationMinutes { get; private set; } = 30; // Varsayılan 30 dk
    
    // Görüşme talebi gerekçesi / konusu (Veli yazar)
    public string RequestNote { get; private set; } = string.Empty;
    
    // Reddedilme veya iptal durumunda eklenecek not
    public string? AdminNote { get; private set; }
    
    // Görüşme sonrasında öğretmen tarafından girilen içerik/rapor notu
    public string? MeetingNotes { get; private set; }
    
    // Görüşme raporu veya ek dosya URL'i (PDF veya Görsel)
    public string? AttachmentUrl { get; private set; }
    
    // Görüşme talebi durumu
    public MeetingStatus Status { get; private set; } = MeetingStatus.Pending;
    
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; private set; }

    // EF Core için boş ctor
    private MeetingRequest() { }

    public MeetingRequest(Guid childId, Guid parentId, Guid teacherId, DateTime plannedDate, string requestNote, int durationMinutes = 30)
    {
        ChildId = childId;
        ParentId = parentId;
        TeacherId = teacherId;
        PlannedDate = plannedDate;
        RequestNote = requestNote;
        DurationMinutes = durationMinutes;
        Status = MeetingStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public void Approve(DateTime plannedDate, string? adminNote = null)
    {
        PlannedDate = plannedDate;
        AdminNote = adminNote;
        Status = MeetingStatus.Approved;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Reject(string? adminNote = null)
    {
        AdminNote = adminNote;
        Status = MeetingStatus.Rejected;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel(string? adminNote = null)
    {
        AdminNote = adminNote;
        Status = MeetingStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Complete(string meetingNotes, string? attachmentUrl = null)
    {
        MeetingNotes = meetingNotes;
        AttachmentUrl = attachmentUrl;
        Status = MeetingStatus.Completed;
        UpdatedAt = DateTime.UtcNow;
    }
}
