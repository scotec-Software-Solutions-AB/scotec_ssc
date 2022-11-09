using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using Scotec.Extensions.Linq;

namespace Scotec.Smtp.Service;

public class EmailClient : IEmailClient
{
    private const string SMTP_SERVER_SECTION = "SMTPServer";
    private readonly ILogger<EmailClient> _logger;
    private readonly SmtpServerConfiguration _smtpServerConfiguration;

    public EmailClient(IConfiguration configuration, ILogger<EmailClient> logger)
    {
        _logger = logger;
        _smtpServerConfiguration = configuration?.GetSection(SMTP_SERVER_SECTION).Get<SmtpServerConfiguration>();
    }

    public bool SendEmail(EmailMessage email)
    {
        if (!_smtpServerConfiguration.SendingEnabled)
            return true;

        using var message = new MimeMessage();
        message.From.Add(email.From);
        email.To.ForAll(to => message.To.Add(to));
        email.Cc.ForAll(cc => message.Cc.Add(cc));
        email.Bcc.ForAll(bcc => message.Bcc.Add(bcc));
        message.Subject = email.Subject;
        message.Body = new TextPart("plain")
        {
            Text = email.Body
        };

        //if (email.Attachment != null)
        //{
        //    message.Attachments.Add(new Attachment(new MemoryStream(email.Attachment), "Attachment.name", "text/???"));
        //}
        try
        {
            using var smtpClient = CreateSmtpClient();

            smtpClient.Send(message);
            smtpClient.Disconnect(true);
        }
        catch (Exception e)
        {
            _logger.LogWarning($"Could not send email. Reason: {e.Message}");
            return false;
        }

        return true;
    }

    private SmtpClient CreateSmtpClient()
    {
        var client = new SmtpClient();
        //client.LocalDomain = "scotec-software.com";
        client.Connect(_smtpServerConfiguration.Server, _smtpServerConfiguration.Port, SecureSocketOptions.StartTls);
        client.Authenticate(_smtpServerConfiguration.LoginUsername, _smtpServerConfiguration.LoginPassword);

        return client;
    }
}