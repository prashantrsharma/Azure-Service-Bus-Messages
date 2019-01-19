using System;
using System.Collections.Generic;
using System.Text;

namespace TopicsAndSubscriptionsAtWork.Configuration
{
    public class Settings
    {
        public static string ConnectionString = "Endpoint=sb://simplebrokeredmessaging.servicebus.windows.net;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=fpIbQ/oyupGgm7TYPIvstdYbFbuGmW41tVacrDfZfbU=";
        public static string TopicName = "PizzaTopic";
        public static string PizzaSubscription = "PizzaSubscription";
        public static string PizzaSubscriptionWithMeat = "PizzaSubscriptionWithMeat";
    }
}
