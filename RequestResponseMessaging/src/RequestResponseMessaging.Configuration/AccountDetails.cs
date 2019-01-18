using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequestResponseMessaging.Configuration
{
    public class AccountDetails
    {
        public static string ConnectionString = "Endpoint=sb://simplebrokeredmessaging.servicebus.windows.net;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=fpIbQ/oyupGgm7TYPIvstdYbFbuGmW41tVacrDfZfbU=";
        public static string RequestQueueName = "requestqueue";
        public static string ResponseQueueName = "responsequeue";
    }
}
