using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrorHandling.Config
{
    public class ConfigurationSettings
    {
        public static string ConnectionString = "Endpoint=sb://simplebrokeredmessaging.servicebus.windows.net;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=fpIbQ/oyupGgm7TYPIvstdYbFbuGmW41tVacrDfZfbU=";
        public static string TopicName = "errorhandling";
        public static string QueueName = "transientfaults";
        public static string BadConnectionString = "";
    }
}
