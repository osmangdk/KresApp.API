namespace KresApp.Application.Interfaces;

public interface IJwtService
{
    string Generate(Guid userId, string role, string email);
}