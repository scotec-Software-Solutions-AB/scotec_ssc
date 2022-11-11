using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Scotec.Smtp.Service.NoBuffer;

/// <summary>
/// This implementation does not read messages from an email buffer. Instead it is called directly from <see cref="NoBufferEmailBuffer"/>
/// </summary>
public class NoBufferEmailDispatcher : INoBufferEmailDispatcher
{
    private readonly IServiceProvider _services;
    private readonly ILogger<NoBufferEmailDispatcher> _logger;

    public NoBufferEmailDispatcher(IServiceProvider services, ILogger<NoBufferEmailDispatcher> logger)
    {
        _services = services;
        _logger = logger;
    }

    public void SendEmail(EmailMessage email)
    {
        var emailClient = _services.GetService<IEmailClient>();

        if (emailClient == null)
        {
            var error = "Could not find service of type 'IEmailClient'.";
            _logger.LogError(error);

            throw new SmtpServiceException(error);
        }

        emailClient.SendEmail(email);
    }
}