using KresApp.Domain.Entities;
using KresApp.Domain.Enums;

namespace KresApp.Application.DTOs;

// LDAP'tan geçince kullanıcı talep gönderir (rol seçmez)
public record CreateAccessRequestDto(
    string Email,
    string Name,
    string? Phone
);

// Admin listede görür
public record AccessRequestDto(
    Guid Id,
    string Email,
    string Name,
    string? Phone,
    string Status,
    DateTime CreatedAt
);

// Admin onaylarken rol atar
public record ApproveAccessRequestDto(
    UserRole Role,
    string? AdminNote
);
