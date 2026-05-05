using KresApp.Domain.Enums;

namespace KresApp.Domain.Entities;

public class Payment
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid ChildId { get; private set; }
    public decimal Amount { get; private set; }
    public int Month { get; private set; }
    public int Year { get; private set; }
    public PaymentStatus Status { get; private set; }
    public DateTime DueDate { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Payment() { }

    public Payment(Guid childId, decimal amount, int month, int year, PaymentStatus status, DateTime dueDate)
    {
        ChildId = childId;
        Amount = amount;
        Month = month;
        Year = year;
        Status = status;
        DueDate = dueDate;
    }

    public void UpdateStatus(PaymentStatus status)
    {
        Status = status;
    }
}
