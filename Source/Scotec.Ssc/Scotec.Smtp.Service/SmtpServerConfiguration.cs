namespace Scotec.Smtp.Service;

public class SmtpServerConfiguration
{
    public bool SendingEnabled { get; set; } = true;
    public string Server { get; set; }
    public int Port { get; set; }
    public string LoginUsername { get; set; }
    public string LoginPassword { get; set; }
    public string From { get; set; } = string.Empty;
    public string To { get; set; } = string.Empty;
}