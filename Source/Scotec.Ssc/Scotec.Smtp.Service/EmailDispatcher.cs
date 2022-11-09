using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Scotec.Smtp.Service;

public class EmailDispatcher : IEmailDispatcher
{
    private ILogger<EmailDispatcher> _logger;
    private readonly IServiceProvider _services;

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