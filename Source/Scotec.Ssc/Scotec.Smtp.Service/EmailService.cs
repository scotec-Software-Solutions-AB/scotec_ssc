using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace Scotec.Smtp.Service
{
    public class EmailService : IEmailService
    {
        private readonly ILogger _logger;
        public IConfiguration _configuration;
        private readonly IEmailBuffer _emailBuffer;

        public EmailService(IEmailBuffer emailBuffer, IConfiguration configuration, ILogger<EmailService> logger)
        {
            _emailBuffer = emailBuffer;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmail(string to, string from, string subject, string body)
        {
            await SendEmail(new Email
            {
                From = from,
                To = to,
                Subject = subject,
                Body = body
            });
        }

        public Task SendEmail(Email email)
        {
            return _emailBuffer.AddEmailAsync(email);
        }
    }
}
