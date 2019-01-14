using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using WorkingWithMessages.Config;

namespace WorkingWithMessages.Forwarder
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Forward";
            Console.WriteLine("Forwarder console.");
            Console.WriteLine();
            CreateQueueIfNotExists();
            ForwardMessages();
            Console.WriteLine("Processing is complete...");
            Console.ReadKey();
        }

        static void ForwardMessages()
        {
            QueueClient inboundClient = QueueClient.CreateFromConnectionString(Settings.ConnectionString, Settings.QueueName);
            QueueClient outboundClient = QueueClient.CreateFromConnectionString(Settings.ConnectionString, Settings.ForwardedQueueName);

            Console.WriteLine("Forwarding messages, hit enter to exit.");

            //Process Message
            inboundClient.OnMessage(message=>
            {
                //Without message cloning
                outboundClient.Send(message);

                //With message cloning
                BrokeredMessage outboundMessage = message.Clone();
                outboundClient.Send(outboundMessage);
                Console.WriteLine($"Forward message:{message.Label}");
            });
        }

        static void CreateQueueIfNotExists()
        {
           NamespaceManager manager = NamespaceManager.CreateFromConnectionString(Settings.ConnectionString);
            if (!manager.QueueExists(Settings.ForwardedQueueName))
            {
                Console.WriteLine($"Creating Queue:{Settings.ForwardedQueueName}");
                manager.CreateQueue(Settings.ForwardedQueueName);
                Console.WriteLine("Done!");
            }

        }
    }
}
