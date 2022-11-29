namespace Scotec.Smtp.Service.NoBuffer;

public interface INoBufferEmailDispatcher : IEmailDispatcher
{
    void SendEmail(EmailMessage email);
}