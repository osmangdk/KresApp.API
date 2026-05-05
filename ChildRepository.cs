using KresApp.Application.Interfaces;
using KresApp.Domain.Entities;
using KresApp.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace KresApp.Persistence.Repositories
{
    public class ChildRepository : IChildRepository
    {
        private readonly AppDbContext _db;

        public ChildRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<Child>> GetByFilter(Guid userId, string role)
        {
            var query = _db.Children.AsQueryable();

            if (role == "Parent")
                query = query.Where(x => x.ParentId == userId);

            return await query.ToListAsync();
        }

        public async Task AddAsync(Child child)
        {
            await _db.Children.AddAsync(child);
            await _db.SaveChangesAsync();
        }
    }
}