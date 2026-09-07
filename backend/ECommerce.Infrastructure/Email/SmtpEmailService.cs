using System.Net;
using System.Net.Mail;
using ECommerce.Domain.Interfaces;
using Microsoft.Extensions.Configuration;

namespace ECommerce.Infrastructure.Email;

public class SmtpEmailService(IConfiguration configuration) : IEmailService
{
    public async Task SendAsync(string toEmail, string toName, string subject, string body, CancellationToken cancellationToken = default)
    {
        var host = configuration["Smtp:Host"] ?? throw new InvalidOperationException("Smtp:Host is not configured.");
        var port = int.Parse(configuration["Smtp:Port"] ?? "587");
        var username = configuration["Smtp:Username"];
        var password = configuration["Smtp:Password"];
        var fromEmail = configuration["Smtp:FromEmail"] ?? username ?? throw new InvalidOperationException("Smtp:FromEmail is not configured.");
        var fromName = configuration["Smtp:FromName"] ?? "ecommerce.store";

        using var client = new SmtpClient(host, port)
        {
            EnableSsl = true,
            Credentials = string.IsNullOrEmpty(username) ? null : new NetworkCredential(username, password),
        };

        using var message = new MailMessage
        {
            From = new MailAddress(fromEmail, fromName),
            Subject = subject,
            Body = body,
        };
        message.To.Add(new MailAddress(toEmail, toName));

        // SmtpClient has no cancellation-aware send overload - honor an
        // already-cancelled token so a request that's given up doesn't still
        // block on a slow SMTP handshake.
        cancellationToken.ThrowIfCancellationRequested();
        await client.SendMailAsync(message);
    }
}
