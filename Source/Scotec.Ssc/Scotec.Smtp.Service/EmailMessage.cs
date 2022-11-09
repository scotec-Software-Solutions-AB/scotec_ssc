namespace Scotec.Smtp.Service;

public sealed class EmailMessage
{
    public EmailAddress From { get; set; }
    public IList<EmailAddress> To { get; } = new List<EmailAddress>();
    public IList<EmailAddress> Cc { get; } = new List<EmailAddress>();
    public IList<EmailAddress> Bcc { get; } = new List<EmailAddress>();
    public string Subject { get; set; }
    public string Body { get; set; }
    public byte[] Attachment { get; set; }
}