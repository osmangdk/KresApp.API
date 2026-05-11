using System;

namespace KresApp.Domain.Entities;

public class SystemSetting
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    // Ön başvuru ayarları
    public bool IsPreEnrollmentActive { get; set; } = true;
    public DateTime? PreEnrollmentStartDate { get; set; }
    public DateTime? PreEnrollmentEndDate { get; set; }
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
