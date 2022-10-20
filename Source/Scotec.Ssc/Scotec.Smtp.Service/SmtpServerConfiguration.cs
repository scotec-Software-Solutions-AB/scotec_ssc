using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scotec.Smtp.Service
{
    public class SmtpServerConfiguration
    {
        public bool SendingEnabled { get; set; }
        public string Server { get; set; }
        public int Port { get; set; }
        public string LoginUsername { get; set; }
        public string LoginPassword { get; set; }
        public string Sender { get; set; }
    }
}
