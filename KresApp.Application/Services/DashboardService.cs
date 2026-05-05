using KresApp.Application.DTOs;
using KresApp.Application.Interfaces;
using KresApp.Domain.Enums;

namespace KresApp.Application.Services;

public class DashboardService
{
    private readonly IChildRepository _childRepo;
    private readonly IPaymentRepository _paymentRepo;
    private readonly IAttendanceRepository _attendanceRepo;
    private readonly IUserRepository _userRepo;

    public DashboardService(IChildRepository childRepo, IPaymentRepository paymentRepo, IAttendanceRepository attendanceRepo, IUserRepository userRepo)
    {
        _childRepo = childRepo;
        _paymentRepo = paymentRepo;
        _attendanceRepo = attendanceRepo;
        _userRepo = userRepo;
    }

    public async Task<DashboardStatsDto> GetStatsAsync(Guid userId, string role)
    {
        var children = await _childRepo.GetByFilter(userId, role);
        int totalStudents = children.Count;

        // Turkey is UTC+3. Using UtcNow.AddHours(3) to ensure correct "today" regardless of server location.
        var today = DateOnly.FromDateTime(DateTime.UtcNow.AddHours(3));
        var attendanceToday = await _attendanceRepo.GetByDateAsync(today, null); 

        var childIds = children.Select(c => c.Id).ToList();
        // Count both Present and Late as "Present" for the dashboard summary
        int presentToday = attendanceToday.Count(a => 
            childIds.Contains(a.ChildId) && 
            (a.Status == AttendanceStatus.Present || a.Status == AttendanceStatus.Late));

        var pendingPayments = await _paymentRepo.GetByFilterAsync(status: PaymentStatus.Pending);
        var overduePayments = await _paymentRepo.GetByFilterAsync(status: PaymentStatus.Overdue);

        if (role == "Parent")
        {
            pendingPayments = pendingPayments.Where(p => childIds.Contains(p.ChildId)).ToList();
            overduePayments = overduePayments.Where(p => childIds.Contains(p.ChildId)).ToList();
        }

        var teachers = await _userRepo.GetByRoleAsync(UserRole.Teacher);
        var admins = await _userRepo.GetByRoleAsync(UserRole.Admin);
        int totalStaff = teachers.Count() + admins.Count();
        
        return new DashboardStatsDto
        {
            TotalStudents = totalStudents,
            PresentToday = presentToday,
            AttendanceRate = totalStudents > 0 ? Math.Round((double)presentToday / totalStudents * 100, 2) : 0,
            PendingPayments = pendingPayments.Count,
            OverduePayments = overduePayments.Count,
            TotalStaff = totalStaff
        };
    }
}
