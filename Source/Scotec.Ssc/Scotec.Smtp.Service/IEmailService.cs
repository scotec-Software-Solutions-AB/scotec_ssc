namespace Scotec.Smtp.Service;

public interface IEmailService
{
    Task SendEmail(EmailMessage email);
}