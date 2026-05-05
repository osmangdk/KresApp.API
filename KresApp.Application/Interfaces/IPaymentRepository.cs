using KresApp.Domain.Entities;
using KresApp.Domain.Enums;

namespace KresApp.Application.Interfaces;

public interface IPaymentRepository
{
    Task<List<Payment>> GetByFilterAsync(Guid? parentId = null, PaymentStatus? status = null, int? month = null, int? year = null);
    Task<Payment?> GetByIdAsync(Guid id);
    Task AddAsync(Payment payment);
    Task UpdateAsync(Payment payment);
}
