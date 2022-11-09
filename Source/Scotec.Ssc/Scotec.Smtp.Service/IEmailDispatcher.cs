namespace Scotec.Smtp.Service;

public interface IEmailDispatcher
{
    bool SendEmail(EmailMessage email);
}