using Microsoft.Extensions.Logging;

namespace Scotec.Smtp.Service;

public class EmailService : IEmailService
{
    private readonly IEmailBuffer _emailBuffer;
    private readonly ILogger _logger;

    public EmailService(IEmailBuffer emailBuffer, ILogger<EmailService> logger)
    {
        _emailBuffer = emailBuffer;
        _logger = logger;
    }

    public Task SendEmail(EmailMessage email)
    {
        _logger.LogTrace("Forwarding email to buffer.");
        return _emailBuffer.AddEmailAsync(email);
    }
}