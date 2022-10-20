using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scotec.Smtp.Service
{
    public sealed class Email
    {
        public Email()
        {
        }

        public string From { get; set; }
        public string To { get; set; }
        public string Cc { get; set; }
        public string Bcc { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public byte[] Attachment { get; set; }
    }
}
