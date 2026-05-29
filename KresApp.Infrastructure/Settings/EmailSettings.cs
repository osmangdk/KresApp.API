namespace KresApp.Infrastructure.Settings;

public class EmailSettings
{
    public string SmtpServer { get; set; } = "smtp.aile.gov.tr";
    public int SmtpPort { get; set; } = 587;
    public string SenderEmail { get; set; } = "info@aile.gov.tr";
    public string SenderName { get; set; } = "ASHB Kreş Yönetim Sistemi";
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public bool EnableSsl { get; set; } = true;
}
