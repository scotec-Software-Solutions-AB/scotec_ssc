using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace Scotec.Smtp.Service
{
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

        public bool SendEmail(Email email)
        {
            if (!_smtpServerConfiguration.SendingEnabled)
                return true;

            using var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Contact", email.From ?? _smtpServerConfiguration.LoginUsername));
            message.To.Add(new MailboxAddress("Olaf Meyer", email.To ?? _smtpServerConfiguration.LoginUsername));
            message.Subject = email.Subject;
            message.Body = new TextPart("plain")
            {
                Text = email.Body
            };

            //if (email.Attachment != null)
            //{
            //    message.Attachments.Add(new Attachment(new MemoryStream(email.Attachment), "Attachment.name", "text/???"));
            //}
            if (!string.IsNullOrEmpty(email.Cc))
            {
                message.Cc.Add(new MailboxAddress("", email.Cc));
            }

            if (!string.IsNullOrEmpty(email.Bcc))
            {
                message.Bcc.Add(new MailboxAddress("", email.Bcc));
            }

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
}
