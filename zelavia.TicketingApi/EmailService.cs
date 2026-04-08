using MimeKit;
using MailKit.Net.Smtp;

namespace zelavia.TicketingApi;

public class EmailService
{
    private readonly string _smtpHost;
    private readonly int _smtpPort;

    public EmailService(IConfiguration configuration)
    {
        _smtpHost = configuration.GetConnectionString("mailpit")?.Split(':')[0] ?? "localhost";
        _smtpPort = 1025;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        using var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Sender", "sender@example.com"));
        message.To.Add(new MailboxAddress("Recipient", to));
        message.Subject = subject;
        var bb = new BodyBuilder();
        bb.TextBody = "This is a test email.";
        bb.HtmlBody = $"<h1>Hello World</h1><p>This is <b>HTML</b> email.</p> <br>{body}";
        bb.Attachments.Add("Dometrain.png");

        message.Body = bb.ToMessageBody();


        using var client = new SmtpClient();
        await client.ConnectAsync(_smtpHost, _smtpPort, false);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}
