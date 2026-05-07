namespace KresApp.Infrastructure.Settings;

public class LdapSettings
{
    public bool Enabled { get; set; }
    public AdDomainSetting Domains { get; set; } = new();
}

public class AdDomainSetting
{
    public string Name { get; set; } = string.Empty; // e.g. aile.bulutu
    public string Container { get; set; } = string.Empty; // e.g. DC=aile,DC=bulutu
}
