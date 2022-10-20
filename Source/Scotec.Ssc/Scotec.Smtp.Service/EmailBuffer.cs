
using Microsoft.Extensions.Logging;

namespace Scotec.Smtp.Service
{
    /// <summary>
    /// This buffer does not buffer emails at all. 
    /// Instead incoming email are directly forwardet to the email dispatcher.
    /// </summary>
    public class EmailBuffer : IEmailBuffer
    {
        private IEmailDispatcher _emailDispatcher;
        private ILogger<EmailBuffer> _logger;
        public EmailBuffer(IEmailDispatcher emailDispatcher, ILogger<EmailBuffer> logger)
        {
            _emailDispatcher = emailDispatcher;
            _logger = logger;
        }

        public async Task AddEmailAsync(Email email)
        {
            await Task.Run(() =>
            {
                _emailDispatcher.SendEmail(email);
            });
        }
    }
}
