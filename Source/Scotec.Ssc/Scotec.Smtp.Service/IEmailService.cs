namespace Scotec.Smtp.Service;

/// <summary>
///     The Email Service is used to forward a message to the Email Buffer.
/// </summary>
public interface IEmailService
{
    Task SendEmail(EmailMessage email);
}