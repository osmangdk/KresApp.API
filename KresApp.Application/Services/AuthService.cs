using KresApp.Application.Interfaces;
using KresApp.Domain.Entities;
using KresApp.Domain.Enums;

namespace KresApp.Application.Services;

public class AuthService
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtService _jwt;
    private readonly ILdapService _ldap;

    public AuthService(IUserRepository users, IPasswordHasher hasher, IJwtService jwt, ILdapService ldap)
    {
        _users = users;
        _hasher = hasher;
        _jwt = jwt;
        _ldap = ldap;
    }

    public async Task Register(string email, string password, UserRole role, string name = "", string? phone = null)
    {
        var existing = await _users.GetByEmail(email);
        if (existing != null)
            throw new Exception("User already exists");

        var hash = _hasher.Hash(password);
        var user = new User(email, hash, role, name, phone);
        await _users.AddAsync(user);
    }

    /// <summary>
    /// Sadece LDAP doğrulaması yapar. AccessRequestController tarafından kullanılır.
    /// </summary>
    public async Task<bool> ValidateLdapAsync(string email, string password)
    {
        try { return await _ldap.AuthenticateAsync(email, password); }
        catch { return false; }
    }

    /// <summary>
    /// Login sonucu: token döner ya da "not_registered" status döner.
    /// </summary>
    public async Task<LoginResult> Login(string email, string password)
    {
        var user = await _users.GetByEmail(email);

        // 1. LDAP denemesi — bağlantı hatası veya timeout olursa yerel auth'a düş
        bool isLdapAuth = false;
        try { isLdapAuth = await _ldap.AuthenticateAsync(email, password); }
        catch { isLdapAuth = false; }

        if (isLdapAuth)
        {
            if (user == null)
            {
                // LDAP'ta var ama sistemde hesabı yok → talep ekranına yönlendir
                LdapUserInfo? info = null;
                try { info = await _ldap.GetUserInfoAsync(email); } catch { }

                return new LoginResult(
                    Status: "not_registered",
                    Token: null,
                    LdapName: info?.Name ?? email
                );
            }
            return new LoginResult("success", _jwt.Generate(user.Id, user.Role.ToString(), user.Email), null);
        }

        // 2. LDAP başarısız veya devre dışı — yerel veritabanı kontrolü
        if (user == null)
            throw new Exception("E-posta veya şifre hatalı.");

        if (user.PasswordHash == "LDAP_USER")
            throw new Exception("Kurumsal hesabınızla giriş yapılamadı. Ağa bağlı olduğunuzdan emin olun.");

        var ok = _hasher.Verify(password, user.PasswordHash);
        if (!ok)
            throw new Exception("E-posta veya şifre hatalı.");

        return new LoginResult("success", _jwt.Generate(user.Id, user.Role.ToString(), user.Email), null);
    }
}

public record LoginResult(string Status, string? Token, string? LdapName);