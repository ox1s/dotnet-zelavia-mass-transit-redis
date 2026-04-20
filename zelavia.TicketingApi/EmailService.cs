using MimeKit;
using MailKit.Net.Smtp;

namespace zelavia.TicketingApi;

public class EmailService
{
    private readonly string _smtpHost;
    private readonly int _smptPort;

    public EmailService(IConfiguration configuration)
    {
        var values = configuration.GetConnectionString("mailpit")?.Split(':');
        _smtpHost = values[1].Replace("//", "") ?? "localhost";
        _smptPort = Convert.ToInt32(values[2]); 
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        using var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Sender", "sender@example.com"));
        message.To.Add(new MailboxAddress("Recipient", to));
        message.Subject = subject;
        var bb = new BodyBuilder();
        bb.TextBody = "TICKET";
        bb.HtmlBody = $"<h1>This is your ticket</h1><p>Zelavia<b>Inno</b><br>{body}";

        message.Body = bb.ToMessageBody();


        using var client = new SmtpClient();
        await client.ConnectAsync(_smtpHost, _smptPort, false);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}
