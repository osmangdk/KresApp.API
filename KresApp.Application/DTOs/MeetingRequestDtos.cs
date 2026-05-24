using System;

namespace KresApp.Application.DTOs;

public class MeetingRequestDto
{
    public Guid Id { get; set; }
    public Guid ChildId { get; set; }
    public string ChildFullName { get; set; } = string.Empty;
    public Guid ParentId { get; set; }
    public string ParentName { get; set; } = string.Empty;
    public Guid TeacherId { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public DateTime PlannedDate { get; set; }
    public int DurationMinutes { get; set; }
    public string RequestNote { get; set; } = string.Empty;
    public string? AdminNote { get; set; }
    public string? MeetingNotes { get; set; }
    public string? AttachmentUrl { get; set; }
    public int Status { get; set; }
    public string StatusText { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class CreateMeetingRequestDto
{
    public Guid ChildId { get; set; }
    public Guid TeacherId { get; set; }
    public DateTime PlannedDate { get; set; }
    public string RequestNote { get; set; } = string.Empty;
    public int DurationMinutes { get; set; } = 30;
}

public class ApproveMeetingRequestDto
{
    public DateTime PlannedDate { get; set; }
    public string? AdminNote { get; set; }
}

public class RejectMeetingRequestDto
{
    public string? AdminNote { get; set; }
}

public class CompleteMeetingRequestDto
{
    public string MeetingNotes { get; set; } = string.Empty;
    public string? AttachmentUrl { get; set; }
}
