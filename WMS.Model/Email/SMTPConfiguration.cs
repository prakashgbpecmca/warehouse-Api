using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS.Model.Email
{
    public class SMTPConfiguration
    {

        public string Subject { get; set; }
        public string MailContent { get; set; }
        public string HeaderLogo { get; set; }
        public string Attachments { get; set; }

        public string To { get; set; }
        public string CC { get; set; }
        public string BCC { get; set; }

        public string MailHostName { get; set; }
        public string SMTPport { get; set; }
        public string SMTPUserName { get; set; }
        public string SMTPPassword { get; set; }
        public string EnableSsl { get; set; } 
        public string FromMail { get; set; }
        public string DisplayName { get; set; }
    }
}
