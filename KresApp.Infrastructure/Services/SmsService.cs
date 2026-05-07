using KresApp.Application.Interfaces;
using KresApp.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using System.Text;

namespace KresApp.Infrastructure.Services;

public class SmsService : ISmsService
{
    private readonly SmsSettings _settings;
    private readonly HttpClient _httpClient;

    public SmsService(IOptions<SmsSettings> settings)
    {
        _settings = settings.Value;
        _httpClient = new HttpClient();
    }

    public async Task<bool> SendSmsAsync(string phoneNumber, string message)
    {
        try
        {
            // ÖRNEK TTNET SMS ENTEGRASYONU (XML TABANLI)
            // Not: Gerçek URL ve parametreleri dokümanınıza göre güncelleyiniz.
            
            var xmlData = $@"
                <SMS>
                    <AUTHENTICATION>
                        <USERNAME>{_settings.Username}</USERNAME>
                        <PASSWORD>{_settings.Password}</PASSWORD>
                    </AUTHENTICATION>
                    <ORDER>
                        <SENDER>{_settings.Header}</SENDER>
                        <SENDDATE></SENDDATE>
                        <MESSAGE>
                            <TEXT>{message}</TEXT>
                            <RECEIVER>{phoneNumber}</RECEIVER>
                        </MESSAGE>
                    </ORDER>
                </SMS>";

            var content = new StringContent(xmlData, Encoding.UTF8, "application/xml");
            var response = await _httpClient.PostAsync(_settings.ApiUrl, content);

            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
