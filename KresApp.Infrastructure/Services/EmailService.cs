using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using KresApp.Application.Interfaces;
using KresApp.Domain.Entities;
using KresApp.Infrastructure.Settings;
using Microsoft.Extensions.Options;

namespace KresApp.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;
    private readonly IEmailLogRepository _emailLogRepo;

    public EmailService(IOptions<EmailSettings> settings, IEmailLogRepository emailLogRepo)
    {
        _settings = settings.Value;
        _emailLogRepo = emailLogRepo;
    }

    public async Task<bool> SendEmailAsync(string toEmail, string subject, string body)
    {
        Console.WriteLine($"\n[SMTP MAIL] Recipient: {toEmail} | Subject: {subject}");
        Console.WriteLine($"[SMTP MAIL Body]: {body}\n");

        bool isSuccess = false;
        string? errorMessage = null;

        try
        {
            if (string.IsNullOrWhiteSpace(_settings.SmtpServer) || 
                (_settings.SmtpServer == "smtp.aile.gov.tr" && string.IsNullOrWhiteSpace(_settings.Username)))
            {
                Console.WriteLine("[SMTP MOCK] Email successfully simulated (SMTP server or credentials empty).");
                isSuccess = true;
            }
            else
            {
                using var client = new SmtpClient(_settings.SmtpServer, _settings.SmtpPort)
                {
                    EnableSsl = _settings.EnableSsl
                };

                if (!string.IsNullOrWhiteSpace(_settings.Username))
                {
                    client.Credentials = new NetworkCredential(_settings.Username, _settings.Password);
                }

                using var mailMessage = new MailMessage
                {
                    From = new MailAddress(_settings.SenderEmail, _settings.SenderName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);

                await client.SendMailAsync(mailMessage);
                Console.WriteLine("[SMTP SUCCESS] Email delivered successfully.");
                isSuccess = true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SMTP ERROR] Failed to send email: {ex.Message}");
            errorMessage = ex.Message;
            isSuccess = false;
        }
        finally
        {
            try
            {
                var log = new EmailLog
                {
                    ToEmail = toEmail,
                    Subject = subject,
                    Body = body,
                    IsSuccess = isSuccess,
                    ErrorMessage = errorMessage,
                    SentAt = DateTime.UtcNow
                };
                await _emailLogRepo.AddAsync(log);
            }
            catch (Exception logEx)
            {
                Console.WriteLine($"[SMTP LOG ERROR] Failed to write email log to DB: {logEx.Message}");
            }
        }

        return isSuccess;
    }
}
