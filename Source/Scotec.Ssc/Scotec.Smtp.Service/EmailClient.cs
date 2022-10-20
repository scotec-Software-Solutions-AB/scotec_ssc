using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;

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

            // receivers need to be separated by commas
            using var message = new MailMessage
            {
                From = new MailAddress(email.From),
                To = { email.To },
                Subject = email.Subject,
                Body = email.Body,
            };
            if (email.Attachment != null)
            {
                message.Attachments.Add(new Attachment(new MemoryStream(email.Attachment), "Attachment.name", "text/calendar"));
            }
            if (!string.IsNullOrEmpty(email.Cc))
            {
                message.CC.Add(email.Cc);
            }

            if (!string.IsNullOrEmpty(email.Bcc))
            {
                message.Bcc.Add(email.Bcc);
            }

            message.IsBodyHtml = true;
            //mail.AlternateViews.Add(GetEmbeddedImage("logo.jpg", mail.Body));

            try
            {
                using var smtpClient = CreateSmtpClient();
                smtpClient.Send(message);
            }
            catch (SmtpException e)
            {
                _logger.LogWarning($"Could not send email. Reason: {e.Message}");
                return false;
            }

            return true;
        }

        private SmtpClient CreateSmtpClient()
        {
            var client = new SmtpClient
            {
                Host = _smtpServerConfiguration.Server,
                Port = _smtpServerConfiguration.Port,
                Credentials = new NetworkCredential
                {
                    UserName = _smtpServerConfiguration.LoginUsername,
                    Password = _smtpServerConfiguration.LoginPassword
                },
                EnableSsl = true
            };
            return client;
        }
    }
}
