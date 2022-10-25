using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationServices.Core.AppSettings
{
    public class NotificationSettings
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string From { get; set; }
        public string SenderTitle { get; set; }
        public string ReceiverTitle { get; set; }
        public string Password { get; set; }
    }
}
