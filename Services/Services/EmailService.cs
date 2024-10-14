using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Services.Interfaces;
using Services.ServiceModels;
using System.Threading.Tasks;
using System;
using static Services.Exceptions.EmailExceptions;

public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;
    private readonly IEmailQueue _emailQueue;

    public EmailService(IOptions<EmailSettings> emailSettings, IEmailQueue emailQueue)
    {
        _emailSettings = emailSettings.Value;
        _emailQueue = emailQueue;
    }

    public void SendEmail(string toEmail, string subject, string body)
    {
        var emailMessage = new EmailMessage
        {
            ToEmail = toEmail,
            Subject = subject,
            Body = body
        };

        _emailQueue.EnqueueEmail(emailMessage);
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

        using var smtp = new SmtpClient();
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
