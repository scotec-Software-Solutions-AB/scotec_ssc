using MailKit.Net.Smtp;
using MailKit.Security;

namespace Scotec.Smtp.Service.Test
{
    internal class Program
    {

        static readonly string Server = "smtp.office365.com";
        static readonly int Port = 587;
        static readonly string LoginUsername = "USERNAME";
        static readonly string LoginPassword = "PASSWORD";
        static readonly string From = "FROM_MAIL";
        static readonly string To = "TO_MAIL";


        static void Main(string[] args)
        {
            var client = CreateSmtpClient();       }

        private static SmtpClient CreateSmtpClient()
        {
            var client = new SmtpClient();
            //client.LocalDomain = "scotec-software.com";
            client.Connect(Server, Port, SecureSocketOptions.StartTls);
            client.Authenticate(LoginUsername, LoginPassword);

            return client;
        }

    }
}
