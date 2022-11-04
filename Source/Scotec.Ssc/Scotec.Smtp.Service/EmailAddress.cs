using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace Scotec.Smtp.Service
{
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

        public static implicit operator MailboxAddress(EmailAddress address) => new (address.Name, address.Email);
    }
}
