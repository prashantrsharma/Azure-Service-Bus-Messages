using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace TopicsAndSubscriptions.WireTap
{
    class Program
    {
        private static readonly string ConnectionString = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
        private const string TopicPath = "orderTopic";
        private const string WireTap = "wiretap-";

        static void Main(string[] args)
        {
            Console.Title = "Wire Tap Console...";
            Console.WriteLine("Hit enter to start wiretap");
            Console.ReadLine();

            //Create a  namespace manager
            NamespaceManager manager = NamespaceManager.CreateFromConnectionString(ConnectionString);

            //Create a new subscription
            string subscriptionName = string.Concat(WireTap, Guid.NewGuid().ToString("N"));
            manager.CreateSubscription(new SubscriptionDescription(TopicPath, subscriptionName)
            {
                AutoDeleteOnIdle = TimeSpan.FromMinutes(5)
            });

            //Create a new subscription client
            SubscriptionClient client = SubscriptionClient.CreateFromConnectionString(ConnectionString,TopicPath,subscriptionName);

            //Receive messages and display properties
            client.OnMessage(message =>
            {
                Console.WriteLine("Message received.");
                foreach (KeyValuePair<string,object> msgProperty in message.Properties)
                {
                    Console.WriteLine($"{msgProperty.Key} - {msgProperty.Value}");
                }
                Console.WriteLine();
            });
            Console.WriteLine("Wire tap running.......");
            Console.ReadLine();
        }
    }
}
