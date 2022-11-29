namespace Scotec.Smtp.Service;

public class SmtpServiceException : ApplicationException
{
    public SmtpServiceException()
    {
    }

    public SmtpServiceException(string message)
        : base(message)
    {
    }

    public SmtpServiceException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}