using KresApp.Domain.Enums;

namespace KresApp.Application.DTOs;

public class PaymentDto
{
    public Guid Id { get; set; }
    public Guid ChildId { get; set; }
    public string ChildName { get; set; } = null!;
    public decimal Amount { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public PaymentStatus Status { get; set; }
    public DateTime DueDate { get; set; }
}

public class CreatePaymentDto
{
    public Guid ChildId { get; set; }
    public decimal Amount { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public DateTime DueDate { get; set; }
}

public class PaymentSummaryDto
{
    public decimal Total { get; set; }
    public decimal Collected { get; set; }
    public decimal Pending { get; set; }
    public int PaidCount { get; set; }
    public int TotalCount { get; set; }
}
