namespace Scotec.Smtp.Service;

public interface IEmailClient
{
    /// <summary>
    ///     Establishes a connection to the smtp server and sends an email.
    /// </summary>
    /// <param name="email">Message to send.</param>
    /// <returns>Return true if the message could be successfully passed to the smtp server, otherwise false.</returns>
    /// <remarks>
    ///     The caller of this method should handle the return value. In case of an error, the caller should not remove the
    ///     message from the buffer.
    ///     Instead it should call this method again after some waiting time.
    /// </remarks>
    bool SendEmail(EmailMessage email);
}