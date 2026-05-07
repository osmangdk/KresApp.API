namespace KresApp.Infrastructure.Settings;

public class SmsSettings
{
    public string ApiUrl { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Header { get; set; } = string.Empty; // Title of the SMS
}
