using MailKit.Net.Smtp;
using MimeKit;
using Services.Interfaces;

public class GmailEmailService : IEmailService
{
    private readonly IConfiguration _config;

    public GmailEmailService(IConfiguration config)
    {
        _config = config;

    }

    public async Task SendAsync(string subject, string content)
    {
        var userName = _config["EmailSettings:Username"];
        var password = _config["EmailSettings:Password"];
        var smtpHost = _config["EmailSettings:SmtpHost"];
        var port = _config.GetValue<int>("EmailSettings:Port");
        var ToEmail = _config["EmailSettings:ToEmail"];

        var message = new MimeMessage();


        message.From.Add(MailboxAddress.Parse(userName));
        message.To.Add(MailboxAddress.Parse(ToEmail));

        message.Subject = subject;

        message.Body = new TextPart("html")
        {
            Text = content
        };

        using var client = new SmtpClient();

        client.CheckCertificateRevocation = false;

        await client.ConnectAsync(smtpHost, port, MailKit.Security.SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(userName, password);

        await client.SendAsync(message);

        await client.DisconnectAsync(true);
    }
}