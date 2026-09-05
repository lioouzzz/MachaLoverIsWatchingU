using MailKit.Net.Smtp;
using MimeKit;
using Services.Interfaces;
using System.Net.Sockets;

public class GmailEmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<GmailEmailService> _logger;
    public GmailEmailService(IConfiguration config, ILogger<GmailEmailService> logger)
    {
        _config = config;
        _logger = logger;
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


        //重試機制
        const int maxRetry = 3;

        for (int retry = 1; retry < maxRetry; retry++)
        {
            try
            {
                await client.SendAsync(message);

                _logger.LogInformation("Email 寄送成功，第 {retry} 次嘗試", retry);
                return;   //成功發郵件，就退出重試迴圈
            }
            catch (SmtpCommandException ex)
            {
                var isTransient = (int)ex.StatusCode >= 400 && (int)ex.StatusCode < 500;

                if (!isTransient || retry == maxRetry)
                {
                    throw;
                }

                _logger.LogWarning(ex, "SMTP 暫時性錯誤，第 {retry} 次寄送失敗", retry);

            }
            catch (SmtpProtocolException ex)
            {
                if (retry == maxRetry)
                {
                    throw;
                }

                _logger.LogWarning(ex, "SMTP 協議錯誤，第 {retry} 次寄送失敗", retry);
            }
            catch (SocketException ex)
            {
                if (retry == maxRetry)
                {
                    throw;
                }

                _logger.LogWarning(ex, "網路錯誤，第 {retry} 次寄送失敗", retry);

            }

            catch (IOException ex)
            {
                if (retry == maxRetry)
                {
                    throw;
                }

                _logger.LogWarning(ex, "IO 錯誤，第 {retry} 次寄送失敗", retry);
            }

            var delaySeconds = Math.Pow(2, retry);

            await Task.Delay(
                TimeSpan.FromSeconds(delaySeconds));

        }


        await client.DisconnectAsync(true);
    }
}