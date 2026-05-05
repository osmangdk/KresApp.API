using Microsoft.EntityFrameworkCore;
using KresApp.Application.Interfaces;
using KresApp.Domain.Entities;
using KresApp.Persistence.Context;

namespace KresApp.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;
    public UserRepository(AppDbContext db) => _db = db;

    public async Task<User?> GetByIdAsync(Guid id)
        => await _db.Set<User>().FirstOrDefaultAsync(x => x.Id == id);

    public async Task<User?> GetByEmail(string email)
        => await _db.Set<User>().FirstOrDefaultAsync(x => x.Email == email);

    public async Task<IEnumerable<User>> GetByRoleAsync(KresApp.Domain.Enums.UserRole role)
        => await _db.Set<User>().Where(x => x.Role == role).ToListAsync();

    public async Task AddAsync(User user)
    {
        await _db.Set<User>().AddAsync(user);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
        _db.Set<User>().Update(user);
        await _db.SaveChangesAsync();
    }
}