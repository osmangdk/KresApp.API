using System;

namespace KresApp.Application.DTOs;

public record SystemSettingDto(
    bool IsPreEnrollmentActive,
    DateTime? PreEnrollmentStartDate,
    DateTime? PreEnrollmentEndDate,
    DateTime UpdatedAt
);

public record UpdateSystemSettingDto(
    bool IsPreEnrollmentActive,
    DateTime? PreEnrollmentStartDate,
    DateTime? PreEnrollmentEndDate
);
