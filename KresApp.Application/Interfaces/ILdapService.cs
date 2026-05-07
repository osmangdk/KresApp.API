namespace KresApp.Application.Interfaces;

public interface ILdapService
{
    Task<bool> AuthenticateAsync(string email, string password);
    Task<LdapUserInfo?> GetUserInfoAsync(string email);
    Task<bool> IsReachableAsync();
}

public class LdapUserInfo
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
}
