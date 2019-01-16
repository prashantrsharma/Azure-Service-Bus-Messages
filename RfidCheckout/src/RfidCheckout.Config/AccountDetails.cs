using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RfidCheckout.Config
{
    public class AccountDetails
    {
        public static string ConnectionString = "Endpoint=sb://simplebrokeredmessaging.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=fpIbQ/oyupGgm7TYPIvstdYbFbuGmW41tVacrDfZfbU=";
        public static string QueueName = "rfidcheckout";
    }
}
