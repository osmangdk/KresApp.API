using KresApp.Application.Interfaces;
using KresApp.Domain.Entities;
using KresApp.Persistence.Context;

namespace KresApp.Persistence.Repositories;

public class EmailLogRepository : IEmailLogRepository
{
    private readonly AppDbContext _db;

    public EmailLogRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(EmailLog log)
    {
        await _db.Set<EmailLog>().AddAsync(log);
        await _db.SaveChangesAsync();
    }
}
