using Microsoft.Extensions.Logging;

namespace Scotec.Smtp.Service.NoBuffer;

/// <summary>
///     This buffer does not buffer emails at all.
///     Instead incoming email are directly forwardet to the email dispatcher.
/// </summary>
public class NoBufferEmailBuffer : IEmailBuffer
{
    private readonly INoBufferEmailDispatcher _emailDispatcher;
    private readonly ILogger<NoBufferEmailBuffer> _logger;

    public NoBufferEmailBuffer(INoBufferEmailDispatcher emailDispatcher, ILogger<NoBufferEmailBuffer> logger)
    {
        _emailDispatcher = emailDispatcher;
        _logger = logger;
    }

    public Task AddEmailAsync(EmailMessage email)
    {
        Task.Run(() =>
        {
            _logger.LogDebug("Forwarding email message to dispatcher.");

            try
            {
                _emailDispatcher.SendEmail(email);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while forwarding email message to dispatcher. The email could not be sent.");
                _logger.LogWarning("The email buffer will not try to send the message again.");
            }
        });

        return Task.CompletedTask;
    }
}