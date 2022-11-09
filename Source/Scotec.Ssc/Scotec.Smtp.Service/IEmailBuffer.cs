namespace Scotec.Smtp.Service;

public interface IEmailBuffer
{
    Task AddEmailAsync(EmailMessage email);
}