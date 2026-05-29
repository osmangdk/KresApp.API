using System.Threading.Tasks;
using KresApp.Domain.Entities;

namespace KresApp.Application.Interfaces;

public interface IEmailLogRepository
{
    Task AddAsync(EmailLog log);
}
