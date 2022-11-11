using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using Scotec.Extensions.Linq;

namespace Scotec.Smtp.Service;

public class EmailClient : IEmailClient
{
    private const string SmtpServerSection = "SMTPServer";
    private readonly ILogger<EmailClient> _logger;
    private readonly SmtpServerConfiguration _smtpServerConfiguration;

    public EmailClient(IConfiguration configuration, ILogger<EmailClient> logger)
    {
        _logger = logger;
        _smtpServerConfiguration = configuration?.GetSection(SmtpServerSection).Get<SmtpServerConfiguration>();
    }

    [SuppressMessage("ReSharper", "AccessToDisposedClosure")]
    public bool SendEmail(EmailMessage email)
    {
        if (!_smtpServerConfiguration.SendingEnabled)
        {
            var warning = new StringBuilder();
            warning.AppendLine("Sending Mails has been disabled. No messages will be sent to the recipients.");
            warning.Append("Sending should be disabled for debugging and test purposes only.");

            _logger.LogWarning(warning.ToString());
            return true;
        }

        using var message = new MimeMessage();
        message.From.Add(email.From);
        email.To.ForAll(to =>
        {
            Debug.Assert(message != null, nameof(message) + " != null");
            message.To.Add(to);
        });
        email.Cc.ForAll(cc => message.Cc.Add(cc));
        email.Bcc.ForAll(bcc => message.Bcc.Add(bcc));
        message.Subject = email.Subject;
        message.Body = new TextPart("plain")
        {
            Text = email.Body
        };

        //TODO: Add support for attachments.
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