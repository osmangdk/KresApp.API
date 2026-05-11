# Eksik Endpoint'leri Uygulama Planı

## Amaç
`EKSIK_ENDPOINTLER.md` dosyasında belgelenen 44 eksik endpoint'in Clean Architecture yapısına uygun olarak implementasyonu.

## Proposed Changes

### Domain Katmanı

#### [NEW] Entities: `Attendance.cs`, `DailyReport.cs`, `Announcement.cs`, `Payment.cs`, `Conversation.cs`, `Message.cs`, `MealMenu.cs`, `Schedule.cs`
#### [NEW] Enums: `AttendanceStatus.cs`, `MoodStatus.cs`, `MealStatus.cs`, `PaymentStatus.cs`, `AnnouncementCategory.cs`

---

### Application Katmanı

#### [NEW] DTOs: Her modül için request/response DTO'lar
#### [NEW] Interfaces: Her yeni entity için repository interface
#### [NEW] Services: `AttendanceService`, `DailyReportService`, `AnnouncementService`, `PaymentService`, `MessageService`, `MealMenuService`, `ScheduleService`, `DashboardService`, `ProfileService`

---

### Persistence Katmanı

#### [MODIFY] `AppDbContext.cs` — Yeni DbSet'ler ve OnModelCreating konfigürasyonları
#### [NEW] Repositories: Her interface'in PostgreSQL implementasyonu

---

### API Katmanı

#### [NEW] Controllers: `ProfileController`, `AttendanceController`, `DailyReportController`, `AnnouncementsController`, `PaymentsController`, `MessagesController`, `MealMenuController`, `ScheduleController`, `DashboardController`
#### [MODIFY] `ChildrenController.cs` — GET/{id}, PUT, DELETE, PUT allergies, search
#### [MODIFY] `Program.cs` — Yeni DI kayıtları

## Verification Plan

### Automated Tests
- `dotnet build` ile projenin başarıyla derlenmesini doğrula
