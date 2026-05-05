using Microsoft.EntityFrameworkCore;
using KresApp.Application.Interfaces;
using KresApp.Domain.Entities;
using KresApp.Domain.Enums;
using KresApp.Persistence.Context;

namespace KresApp.Persistence.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly AppDbContext _db;

    public PaymentRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Payment>> GetByFilterAsync(Guid? parentId = null, PaymentStatus? status = null, int? month = null, int? year = null)
    {
        var query = _db.Payments.AsQueryable();

        if (parentId.HasValue)
        {
            var childIds = await _db.Children.Where(c => c.ParentId == parentId.Value).Select(c => c.Id).ToListAsync();
            query = query.Where(x => childIds.Contains(x.ChildId));
        }

        if (status.HasValue) query = query.Where(x => x.Status == status.Value);
        if (month.HasValue) query = query.Where(x => x.Month == month.Value);
        if (year.HasValue) query = query.Where(x => x.Year == year.Value);

        return await query.OrderByDescending(x => x.DueDate).ToListAsync();
    }

    public async Task<Payment?> GetByIdAsync(Guid id)
    {
        return await _db.Payments.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task AddAsync(Payment payment)
    {
        await _db.Payments.AddAsync(payment);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Payment payment)
    {
        _db.Payments.Update(payment);
        await _db.SaveChangesAsync();
    }
}
