using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scotec.Smtp.Service
{
    public interface IEmailService
    {
        Task SendEmail(EmailMessage email);
    }
}
