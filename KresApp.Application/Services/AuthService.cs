using KresApp.Application.Interfaces;
using KresApp.Domain.Entities;
using KresApp.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace KresApp.Application.Services;

public class AuthService
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtService _jwt;
    private readonly ILdapService _ldap;
    private readonly ISmsService _sms;
    private readonly IMemoryCache _cache;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _configuration;

    public AuthService(
        IUserRepository users, 
        IPasswordHasher hasher, 
        IJwtService jwt, 
        ILdapService ldap, 
        ISmsService sms, 
        IMemoryCache cache,
        IHttpContextAccessor httpContextAccessor,
        IConfiguration configuration)
    {
        _users = users;
        _hasher = hasher;
        _jwt = jwt;
        _ldap = ldap;
        _sms = sms;
        _cache = cache;
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration;
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
    /// LDAP doğrulaması yapar ve gerekirse SMS gönderir.
    /// </summary>
    public async Task<LdapAuthResult> ValidateLdapWithMfaAsync(string email, string password)
    {
        // IP Adresi Loglama (Hangi IP ile gelindiğini görmek için)
        var remoteIp = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
        Console.WriteLine($"[AUTH_DEBUG] Giriş denemesi yapan IP: {remoteIp}");

        // Sunucuya ulaşılabiliyor mu? (VPN Kontrolü)
        if (!await _ldap.IsReachableAsync())
        {
            throw new Exception("Kurum ağına (aile.bulutu) ulaşılamadı. Lütfen VPN bağlantınızı kontrol edin.");
        }

        bool isLdapAuth = false;
        try { isLdapAuth = await _ldap.AuthenticateAsync(email, password); }
        catch { isLdapAuth = false; }

        if (!isLdapAuth) return new LdapAuthResult(Success: false, RequiresOtp: false);

        // VPN IP kontrolü (Ayarlardan gelen listeye göre)
        var vpnPrefixes = _configuration.GetSection("Auth:VpnIpPrefixes")
            .GetChildren()
            .Select(x => x.Value)
            .Where(x => x != null)
            .ToArray();

        // IPv4-mapped IPv6 durumlarını da (::ffff:172.x.x.x) kapsayacak şekilde kontrol et
        bool isVpn = vpnPrefixes.Any(p => remoteIp != null && (remoteIp.StartsWith(p!) || remoteIp.Contains(":" + p)));

        if (isVpn)
        {
            Console.WriteLine("[AUTH_DEBUG] VPN/Güvenli IP tespit edildi, SMS doğrulaması atlanıyor.");
            return new LdapAuthResult(Success: true, RequiresOtp: false);
        }

        // LDAP BAŞARILI -> SMS GÖNDER
        var otpCode = new Random().Next(100000, 999999).ToString();
        
        // Kullanıcıyı bulup telefonunu al (LDAP'tan veya DB'den)
        var user = await _users.GetByEmail(email);
        var info = await _ldap.GetUserInfoAsync(email);
        var phone = user?.Phone ?? info?.Phone;

        if (string.IsNullOrEmpty(phone))
            throw new Exception("Sistemde kayıtlı telefon numaranız bulunamadı. Lütfen yöneticinizle iletişime geçin.");

        // OTP'yi 3 dakika sakla
        _cache.Set($"otp_{email}", otpCode, TimeSpan.FromMinutes(3));
        
        await _sms.SendSmsAsync(phone, $"KresApp Doğrulama Kodunuz: {otpCode}");

        return new LdapAuthResult(Success: true, RequiresOtp: true);
    }

    /// <summary>
    /// Login sonucu: token döner ya da "not_registered" status döner.
    /// </summary>
    public async Task<LoginResult> Login(string email, string password)
    {
        var user = await _users.GetByEmail(email);

        // 1. LDAP denemesi
        bool isCorporate = email.Contains("@aile.gov.tr") || email.Contains("@aile.bulutu");
        bool isReachable = await _ldap.IsReachableAsync();

        if (!isReachable && isCorporate)
        {
            // Kurumsal email ama sunucuya ulaşılamıyor -> Büyük ihtimalle VPN kapalı
            throw new Exception("Kurum ağına (aile.bulutu) ulaşılamadı. Lütfen VPN bağlantınızın aktif olduğundan emin olun.");
        }

        var ldapResult = await ValidateLdapWithMfaAsync(email, password);
        
        if (ldapResult.Success)
        {
            if (user == null)
            {
                // LDAP doğrulandı ama sistemde hesabı yok → talep ekranına yönlendir
                // Güvenlik için 10 dakikalık bir "LDAP Onaylı" işareti koyuyoruz
                _cache.Set($"ldap_verified_{email}", true, TimeSpan.FromMinutes(10));

                LdapUserInfo? info = null;
                try { info = await _ldap.GetUserInfoAsync(email); } catch { }

                return new LoginResult(
                    Status: "not_registered",
                    Token: null,
                    LdapName: info?.Name ?? email
                );
            }

            if (ldapResult.RequiresOtp)
            {
                return new LoginResult("requires_otp", null, null);
            }

            // SMS gerekmiyorsa (VPN) doğrudan giriş yap ve token dön
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

    public async Task<string> VerifyOtp(string email, string code)
    {
        if (!await VerifyOtpOnly(email, code))
            throw new Exception("Doğrulama kodu hatalı veya süresi dolmuş.");

        var user = await _users.GetByEmail(email);
        if (user == null) throw new Exception("Kullanıcı bulunamadı.");

        return _jwt.Generate(user.Id, user.Role.ToString(), user.Email);
    }

    public async Task<bool> VerifyOtpOnly(string email, string code)
    {
        if (!_cache.TryGetValue($"otp_{email}", out string? savedCode) || savedCode != code)
            return false;

        _cache.Remove($"otp_{email}");
        return true;
    }
}

public record LoginResult(string Status, string? Token, string? LdapName);
public record LdapAuthResult(bool Success, bool RequiresOtp);