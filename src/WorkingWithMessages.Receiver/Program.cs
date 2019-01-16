using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
            //ProcessOrderMessages();
            //SimplePizzaReceiveLoop();
            ReceiveAndProcessPizzaOrdersUsingOnMessage(12);
            //System.Environment.ProcessorCount

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Receiver Console - Complete");
            Console.ReadLine();
        }

        private static void ReceiveAndProcessPizzaOrdersUsingOnMessage(int threads)
        {
            //Create a queue client
            QueueClient client = QueueClient.CreateFromConnectionString(Settings.ConnectionString,Settings.QueueName);

            //Set the Options for using On Message()
            OnMessageOptions options = new OnMessageOptions
            {
                AutoComplete =  false,
                AutoRenewTimeout = TimeSpan.FromSeconds(30),
                MaxConcurrentCalls = threads
            };
            
            //Create a message pump
            client.OnMessage(message =>
            {
                //Deserialize the message body
                PizzaOrder order = message.GetBody<PizzaOrder>();

                //Process the message
                CookPizza(order);

                //Complete the process
                message.Complete();
            },options);

            Console.WriteLine("Receiving,hit enter to exit.");
            Console.ReadLine();
            client.Close();
        }

        static void SimplePizzaReceiveLoop()
        {
           //Create a queue client
           QueueClient client = QueueClient.CreateFromConnectionString(Settings.ConnectionString,Settings.QueueName);

            while (true)
            {
                Console.WriteLine("Receiving........");

                //Receive a message
                BrokeredMessage message = client.Receive(TimeSpan.FromSeconds(5));

                if (message != null)
                {
                    try
                    {
                        //Extract the message
                        PizzaOrder order = message.GetBody<PizzaOrder>();

                        //Process the message
                        CookPizza(order);

                        //Mark the message as complete
                        message.Complete();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Exception:{e.Message}");

                        //Abandon the message
                        message.Abandon();

                        //DeadLetter the message
                        //message.DeadLetter();
                    }
                }
                else
                {
                    Console.WriteLine("No message present on the queue...");
                }

                //Always close the client
               // client.Close();
            }
            
        }

        private static void CookPizza(PizzaOrder order)
        {
            Console.WriteLine($"Cooking {order.Type} for {order.CustomerName}");
            Thread.Sleep(5000);
            Console.WriteLine($"        {order.Type} pizza for {order.CustomerName}");
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
