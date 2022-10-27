using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Scotec.Smtp.Service
{
    public class EmailDispatcher : IEmailDispatcher
    {
        private IServiceProvider _services;
        private ILogger<EmailDispatcher> _logger;

        public EmailDispatcher(IServiceProvider services, ILogger<EmailDispatcher> logger)
        {
            _services = services;
            _logger = logger;
        }

        public bool SendEmail(EmailMessage email)
        {
            var emailClient = _services.GetService<IEmailClient>();
            return emailClient.SendEmail(email);
        }
    }
}
