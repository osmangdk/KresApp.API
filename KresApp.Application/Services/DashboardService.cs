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
    private readonly IClassRepository _classRepo;
    private readonly IEnrollmentRequestRepository _enrollmentRepo;
    private readonly IUserAccessRequestRepository _accessRequestRepo;

    public DashboardService(
        IChildRepository childRepo,
        IPaymentRepository paymentRepo,
        IAttendanceRepository attendanceRepo,
        IUserRepository userRepo,
        IClassRepository classRepo,
        IEnrollmentRequestRepository enrollmentRepo,
        IUserAccessRequestRepository accessRequestRepo)
    {
        _childRepo = childRepo;
        _paymentRepo = paymentRepo;
        _attendanceRepo = attendanceRepo;
        _userRepo = userRepo;
        _classRepo = classRepo;
        _enrollmentRepo = enrollmentRepo;
        _accessRequestRepo = accessRequestRepo;
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
        var parents = await _userRepo.GetByRoleAsync(UserRole.Parent);
        int totalTeachers = teachers.Count();
        int totalParents = parents.Count();
        int totalStaff = totalTeachers + admins.Count();

        var classes = await _classRepo.GetAllAsync();
        int totalClasses = classes.Count;

        var pendingEnrollmentsList = await _enrollmentRepo.GetPendingAsync();
        int pendingEnrollments = pendingEnrollmentsList.Count;

        var pendingAccessList = await _accessRequestRepo.GetAllPendingAsync();
        int pendingAccessRequests = pendingAccessList.Count;

        // Weekly Attendance Calculation
        var weeklyAttendance = new WeeklyAttendanceDto();
        var startOfWeek = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);
        if (today.DayOfWeek == DayOfWeek.Sunday) startOfWeek = today.AddDays(-6);
        
        var endOfWeek = startOfWeek.AddDays(6);
        var weeklyData = await _attendanceRepo.GetByDateRangeAsync(startOfWeek, endOfWeek);

        for (int i = 0; i < 7; i++)
        {
            var date = startOfWeek.AddDays(i);
            var dayAttendance = weeklyData.Where(a => a.Date == date).ToList();
            
            if (dayAttendance.Count == 0 || totalStudents == 0)
            {
                weeklyAttendance.Rates.Add(0);
            }
            else
            {
                int presentCount = dayAttendance.Count(a => 
                    a.Status == AttendanceStatus.Present || a.Status == AttendanceStatus.Late);
                double rate = Math.Round((double)presentCount / totalStudents * 100, 2);
                weeklyAttendance.Rates.Add(rate);
            }
        }
        
        return new DashboardStatsDto
        {
            TotalStudents = totalStudents,
            PresentToday = presentToday,
            AttendanceRate = totalStudents > 0 ? Math.Round((double)presentToday / totalStudents * 100, 2) : 0,
            PendingPayments = pendingPayments.Count,
            OverduePayments = overduePayments.Count,
            TotalStaff = totalStaff,
            TotalTeachers = totalTeachers,
            TotalParents = totalParents,
            TotalClasses = totalClasses,
            PendingEnrollments = pendingEnrollments,
            PendingAccessRequests = pendingAccessRequests,
            WeeklyAttendance = weeklyAttendance
        };
    }
}
