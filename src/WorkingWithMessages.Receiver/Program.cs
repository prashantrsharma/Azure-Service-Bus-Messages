using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using WorkingWithMessages.Config;
using WorkingWithMessages.DataContracts;

namespace WorkingWithMessages.Receiver
{
    class Program
    {
        static readonly QueueClient Client = QueueClient.CreateFromConnectionString(Settings.ConnectionString, Settings.QueueName);
        static void Main(string[] args)
        {
            Console.Title = "Receiver";
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Receiver Console - Hit Enter");
            Console.ReadLine();
            Console.WriteLine("Receiver Console - Is ready to receive new messages.....");

            //Receive and Process the message
            //ReceiveAndProcess();

            ProcessOrderMessages();

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Receiver Console - Complete");
            Console.ReadLine();
        }

        static void ReceiveAndProcess()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Client.OnMessage(message =>
            {
                Console.WriteLine($"Received:{message.Label}");
            }, new OnMessageOptions { AutoComplete = true });
        }

        static void ProcessOrderMessages()
        {
            QueueClient client = QueueClient.CreateFromConnectionString(Settings.ConnectionString, Settings.QueueName);

            while (true)
            {
                BrokeredMessage message = client.Receive();

                if (message != null)
                {
                    Console.WriteLine("Received message...");

                    //Verify that the message contains Json
                    if (message.ContentType == "application/json")
                    {
                        //Deserialize the message body to a string
                        string content = message.GetBody<string>();
                        Console.WriteLine($"Message Content : {content}");
                        Console.WriteLine($"Message Label : {message.Label}");

                        //Deserialize the JSON string to dynamic type
                        PizzaOrder order = JsonConvert.DeserializeObject<PizzaOrder>(content);
                        Console.WriteLine($"\t {order.CustomerName}");
                        Console.WriteLine($"\t {order.Type}");
                        Console.WriteLine($"\t {order.Size}");
                    }
                    message.Complete();
                }
            }
        }
    }
}
