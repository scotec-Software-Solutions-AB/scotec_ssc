using MimeKit;

namespace Scotec.Smtp.Service;

public class EmailAddress
{
    public EmailAddress(string email)
        : this(email, email)
    {
        Name = email;
        Email = email;
    }

    public EmailAddress(string name, string email)
    {
        Name = name;
        Email = email;
    }

    public string Name { get; set; }
    public string Email { get; set; }

    public static implicit operator MailboxAddress(EmailAddress address)
    {
        return new MailboxAddress(address.Name, address.Email);
    }
}