using KresApp.Application.Interfaces;
using KresApp.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using System.DirectoryServices.AccountManagement;

namespace KresApp.Infrastructure.Services;

public class LdapService : ILdapService
{
    private readonly LdapSettings _settings;

    // LDAP sunucusuna bağlanma için maksimum bekleme süresi (saniye)
    private const int LdapTimeoutSeconds = 20;

    public LdapService(IOptions<LdapSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task<bool> AuthenticateAsync(string email, string password)
    {
        if (!_settings.Enabled) return false;

        // Toplam işlem süresi (VPN/LDAPS yavaştır)
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(LdapTimeoutSeconds));

        try
        {
            var domain = _settings.Domains;
            
            // Farklı kimlik doğrulama yöntemlerini sırayla deneyelim
            var authOptions = new[] {
                ContextOptions.Negotiate | ContextOptions.SecureSocketLayer,
                ContextOptions.SimpleBind | ContextOptions.SecureSocketLayer
            };

            foreach (var options in authOptions)
            {
                if (cts.Token.IsCancellationRequested) break;

                var ldapTask = Task.Run(() =>
                {
                    try 
                    {
                        // Denenecek kullanıcı adları
                        var usernamesToTry = new List<string> { email };
                        if (email.Contains("@"))
                        {
                            usernamesToTry.Add(email.Split('@')[0]);
                        }

                        foreach (var uname in usernamesToTry)
                        {
                            try 
                            {
                                // SSL (636) kullanırken Hostname (aile.bulutu) kullanmak 
                                // sertifika doğrulaması için IP'den daha güvenlidir.
                                using var ctx = new PrincipalContext(
                                    ContextType.Domain,
                                    domain.Name,
                                    domain.Container,
                                    options,
                                    uname, // Try binding with uname instead of strictly email
                                    password);
                                    
                                if (ctx.ValidateCredentials(uname, password))
                                    return true;
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"[LDAP_ERROR] Context/Validate failed for {uname} with options {options}: {ex.Message}");
                                // Continue to the next username format
                            }
                        }
                        
                        return false;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[LDAP_ERROR] AuthenticateAsync failed unexpectedly: {ex.Message}");
                        return false;
                    }
                }, cts.Token);

                try 
                {
                    // Her bir deneme için 10 saniye limit
                    if (await ldapTask.WaitAsync(TimeSpan.FromSeconds(10), cts.Token)) 
                        return true;
                }
                catch 
                { 
                    continue; 
                }
            }

            return false;
        }
        catch
        {
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
                try 
                {
                    var options = ContextOptions.Negotiate;
                    if (domain.UseSsl) options |= ContextOptions.SecureSocketLayer;

                    var server = domain.Port != 389 && domain.Port != 636 
                        ? $"{domain.Name}:{domain.Port}" 
                        : domain.Name;

                    using var ctx = new PrincipalContext(ContextType.Domain, server, domain.Container, options);

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
                }
                catch
                {
                    return null;
                }
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

    public async Task<bool> IsReachableAsync()
    {
        if (!_settings.Enabled) return false;

        try
        {
            var domain = _settings.Domains;
            
            // 1. DNS üzerinden tüm IP'leri alalım
            var addresses = await System.Net.Dns.GetHostAddressesAsync(domain.Name);
            if (addresses.Length == 0) return false;

            // 2. IP'leri sırayla deneyelim
            foreach (var ip in addresses)
            {
                try 
                {
                    using var tcpClient = new System.Net.Sockets.TcpClient();
                    var connectTask = tcpClient.ConnectAsync(ip, domain.Port);
                    var timeoutTask = Task.Delay(2000); // IP başına 2 saniye yeterli olmalı

                    if (await Task.WhenAny(connectTask, timeoutTask) == connectTask)
                    {
                        await connectTask; 
                        if (tcpClient.Connected) return true;
                    }
                }
                catch 
                {
                    // Bu IP'ye ulaşılamadı, bir sonrakini dene
                    continue;
                }
            }
            
            return false;
        }
        catch
        {
            return false;
        }
    }
}
