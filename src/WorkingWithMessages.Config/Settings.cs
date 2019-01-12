using System;
using System.Collections.Generic;
using System.Text;

namespace WorkingWithMessages.Config
{
    public class Settings
    {
        public static string ConnectionString = "Endpoint=sb://simplebrokeredmessaging.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=fpIbQ/oyupGgm7TYPIvstdYbFbuGmW41tVacrDfZfbU=";
        public static string QueueName = "workingwithmessages";
        public static string ForwardedQueueName = "forwardedmessages";
        public static string TextString = "The quick brown fox jumps over the lazy dog";
    }
}
