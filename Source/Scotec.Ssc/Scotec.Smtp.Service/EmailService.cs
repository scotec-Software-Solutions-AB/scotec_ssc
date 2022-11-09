using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Scotec.Smtp.Service;

public class EmailService : IEmailService
{
    private readonly IEmailBuffer _emailBuffer;
    private readonly ILogger _logger;
    public IConfiguration _configuration;

    public EmailService(IEmailBuffer emailBuffer, IConfiguration configuration, ILogger<EmailService> logger)
    {
        _emailBuffer = emailBuffer;
        _configuration = configuration;
        _logger = logger;
    }

    public Task SendEmail(EmailMessage email)
    {
        return _emailBuffer.AddEmailAsync(email);
    }
}