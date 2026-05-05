using KresApp.Application.DTOs;
using KresApp.Application.Interfaces;
using KresApp.Domain.Entities;
using KresApp.Domain.Enums;

namespace KresApp.Application.Services;

public class PaymentService
{
    private readonly IPaymentRepository _repo;
    private readonly IChildRepository _childRepo;

    public PaymentService(IPaymentRepository repo, IChildRepository childRepo)
    {
        _repo = repo;
        _childRepo = childRepo;
    }

    public async Task<List<PaymentDto>> GetAsync(Guid? parentId = null, PaymentStatus? status = null, int? month = null, int? year = null)
    {
        var payments = await _repo.GetByFilterAsync(parentId, status, month, year);
        var result = new List<PaymentDto>();

        foreach(var p in payments)
        {
            var child = await _childRepo.GetByIdAsync(p.ChildId);
            result.Add(new PaymentDto
            {
                Id = p.Id,
                ChildId = p.ChildId,
                ChildName = child?.Name ?? "Bilinmeyen",
                Amount = p.Amount,
                Month = p.Month,
                Year = p.Year,
                Status = p.Status,
                DueDate = p.DueDate
            });
        }
        return result;
    }

    public async Task<PaymentDto?> GetByIdAsync(Guid id)
    {
        var p = await _repo.GetByIdAsync(id);
        if (p == null) return null;
        var child = await _childRepo.GetByIdAsync(p.ChildId);
        
        return new PaymentDto
        {
            Id = p.Id, ChildId = p.ChildId, ChildName = child?.Name ?? "",
            Amount = p.Amount, Month = p.Month, Year = p.Year,
            Status = p.Status, DueDate = p.DueDate
        };
    }

    public async Task CreateAsync(CreatePaymentDto dto)
    {
        var payment = new Payment(dto.ChildId, dto.Amount, dto.Month, dto.Year, PaymentStatus.Pending, dto.DueDate);
        await _repo.AddAsync(payment);
    }

    public async Task UpdateStatusAsync(Guid id, PaymentStatus status)
    {
        var payment = await _repo.GetByIdAsync(id);
        if (payment == null) throw new Exception("Payment not found");
        payment.UpdateStatus(status);
        await _repo.UpdateAsync(payment);
    }

    public async Task<PaymentSummaryDto> GetSummaryAsync()
    {
        var all = await _repo.GetByFilterAsync(); // Admin calls this
        return new PaymentSummaryDto
        {
            TotalCount = all.Count,
            PaidCount = all.Count(x => x.Status == PaymentStatus.Paid),
            Total = all.Sum(x => x.Amount),
            Collected = all.Where(x => x.Status == PaymentStatus.Paid).Sum(x => x.Amount),
            Pending = all.Where(x => x.Status == PaymentStatus.Pending || x.Status == PaymentStatus.Overdue).Sum(x => x.Amount)
        };
    }
}
