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

    public async Task<string> Login(string email, string password)
    {
        var user = await _users.GetByEmail(email);

        // 1. LDAP denemesi — bağlantı hatası veya timeout olursa yerel auth'a düş
        bool isLdapAuth = false;
        try
        {
            isLdapAuth = await _ldap.AuthenticateAsync(email, password);
        }
        catch
        {
            // LDAP sunucusuna ulaşılamıyor, yerel auth ile devam edilecek
            isLdapAuth = false;
        }

        if (isLdapAuth)
        {
            if (user == null)
            {
                // LDAP'ta var ama yerelde yoksa "Gölge Kullanıcı" oluştur
                LdapUserInfo? info = null;
                try { info = await _ldap.GetUserInfoAsync(email); } catch { }

                user = new User(email, "LDAP_USER", UserRole.Teacher, info?.Name ?? email, info?.Phone);
                await _users.AddAsync(user);
            }
            return _jwt.Generate(user.Id, user.Role.ToString(), user.Email);
        }

        // 2. LDAP başarısız veya devre dışı — yerel veritabanı kontrolü
        if (user == null)
            throw new Exception("E-posta veya şifre hatalı.");

        if (user.PasswordHash == "LDAP_USER")
        {
            // Sadece LDAP ile giriş yapabilen hesap ama LDAP erişilemiyor
            throw new Exception("Kurumsal hesabınızla giriş yapılamadı. Ağ bağlantınızı kontrol edin.");
        }

        var ok = _hasher.Verify(password, user.PasswordHash);
        if (!ok)
            throw new Exception("E-posta veya şifre hatalı.");

        return _jwt.Generate(user.Id, user.Role.ToString(), user.Email);
    }
}