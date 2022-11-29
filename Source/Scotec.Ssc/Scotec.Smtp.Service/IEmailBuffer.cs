namespace Scotec.Smtp.Service;

public interface IEmailBuffer
{
    /// <summary>
    ///     Adds an email to the buffer. How the email is processed depends on the implementation.
    /// </summary>
    /// <remarks>
    ///     The implementatio of this method shall return as fast as possible. The message can be buffered in memory or in a
    ///     database.
    ///     However, this method shall not establish a connection to the smtp server.
    /// </remarks>
    /// <param name="email">An email message.</param>
    Task AddEmailAsync(EmailMessage email);
}