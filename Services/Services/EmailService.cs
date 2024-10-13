using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Options;
using Services.Interfaces;
using Services.ServiceModels;
using System.Net.Mail;
using System.Threading.Tasks;
using System;
using static Services.Exceptions.EmailExceptions;

public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;

    public EmailService(IOptions<EmailSettings> emailSettings)
    {
        _emailSettings = emailSettings.Value;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var email = new MimeMessage();
        email.Sender = MailboxAddress.Parse($"{_emailSettings.SenderName} <{_emailSettings.SenderEmail}>");
        email.From.Add(MailboxAddress.Parse(_emailSettings.SenderEmail));
        email.To.Add(MailboxAddress.Parse(toEmail));
        email.Subject = subject;

        var builder = new BodyBuilder
        {
            TextBody = body,
            HtmlBody = body.Replace("\r\n", "<br>").Replace("\n", "<br>")
        };
        email.Body = builder.ToMessageBody();

        using var smtp = new MailKit.Net.Smtp.SmtpClient();
        try
        {
            await smtp.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);
            await smtp.SendAsync(email);
        }
        catch (Exception ex)
        {
            throw new EmailException("Something went wrong when sending the email.");
        }
        finally
        {
            await smtp.DisconnectAsync(true);
        }
    }
}
