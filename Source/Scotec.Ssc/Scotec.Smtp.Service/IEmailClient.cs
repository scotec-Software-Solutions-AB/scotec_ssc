namespace Scotec.Smtp.Service;

public interface IEmailClient
{
    bool SendEmail(EmailMessage email);
}