using KresApp.Application.Interfaces;
using KresApp.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using System.DirectoryServices.AccountManagement;

namespace KresApp.Infrastructure.Services;

public class LdapService : ILdapService
{
    private readonly LdapSettings _settings;

    // LDAP sunucusuna bağlanma için maksimum bekleme süresi (saniye)
    private const int LdapTimeoutSeconds = 5;

    public LdapService(IOptions<LdapSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task<bool> AuthenticateAsync(string email, string password)
    {
        if (!_settings.Enabled) return false;

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(LdapTimeoutSeconds));

        try
        {
            var domain = _settings.Domains;

            // PrincipalContext senkron çalışır, Task.Run ile timeout uyguluyoruz
            var ldapTask = Task.Run(() =>
            {
                using var ctx = new PrincipalContext(
                    ContextType.Domain,
                    domain.Name,
                    domain.Container,
                    email,
                    password);
                return ctx.ValidateCredentials(email, password);
            }, cts.Token);

            return await ldapTask.WaitAsync(cts.Token);
        }
        catch (OperationCanceledException)
        {
            // Timeout: LDAP sunucusuna 5 saniyede ulaşılamadı
            return false;
        }
        catch
        {
            // Bağlantı hatası veya kimlik doğrulama başarısız
            return false;
        }
    }

    public async Task<LdapUserInfo?> GetUserInfoAsync(string email)
    {
        if (!_settings.Enabled) return null;

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(LdapTimeoutSeconds));

        try
        {
            var domain = _settings.Domains;

            var ldapTask = Task.Run(() =>
            {
                using var ctx = new PrincipalContext(ContextType.Domain, domain.Name, domain.Container);

                // 1. Önce UserPrincipalName (UPN) ile aramayı dene
                var user = UserPrincipal.FindByIdentity(ctx, IdentityType.UserPrincipalName, email);

                if (user == null)
                {
                    // 2. EmailAddress özelliğine göre arama yap
                    using var userTemplate = new UserPrincipal(ctx) { EmailAddress = email };
                    using var searcher = new PrincipalSearcher(userTemplate);
                    user = searcher.FindOne() as UserPrincipal;
                }

                if (user != null)
                {
                    return new LdapUserInfo
                    {
                        Name = user.DisplayName ?? user.Name,
                        Email = user.EmailAddress ?? email,
                        Phone = user.VoiceTelephoneNumber
                    };
                }

                return null;
            }, cts.Token);

            return await ldapTask.WaitAsync(cts.Token);
        }
        catch (OperationCanceledException)
        {
            // Timeout: LDAP sunucusuna 5 saniyede ulaşılamadı
            return null;
        }
        catch
        {
            return null;
        }
    }
}
