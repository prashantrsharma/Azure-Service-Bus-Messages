using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using TopicsAndSubscriptionsAtWork.Configuration;
using TopicsAndSubscriptionsAtWork.Models;

namespace TopicsAndSubscriptionsAtWork.Receiver
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Receiver-Console";
            Console.WriteLine("Receiver-Console");
            Console.WriteLine("Press any key to proceed......");
            Console.ReadLine();

            //Create a Message Factory
            Console.WriteLine("Create a Message Factory");
            MessagingFactory factory = MessagingFactory.CreateFromConnectionString(Settings.ConnectionString);

            //Create a Topic Subscription Client
            Console.WriteLine("Create a Topic Subscription Client");
            SubscriptionClient client =
                factory.CreateSubscriptionClient(Settings.TopicName, Settings.PizzaSubscriptionWithMeat);

            //Start listening to message with subscription
            Console.WriteLine($"Listening to Topic: {Settings.TopicName} with Subscription {Settings.PizzaSubscriptionWithMeat}");
            Console.ForegroundColor = ConsoleColor.Green;
            //client.OnMessage(msg =>
            //{
            //    //Deserialize the message
            //    PizzaOrder order = msg.GetBody<PizzaOrder>();
                
            //    //Display the message properties
            //    Console.WriteLine($"Message Id# {msg.MessageId}");
            //    Console.WriteLine($"Name : {order.Name}");
            //    Console.WriteLine($"Size : {order.Size}");
            //    Console.WriteLine($"Has Meat : {order.HasMeat}");

            //    Thread.Sleep(TimeSpan.FromMinutes(2));
            //    //Complete the message
            //    msg.Complete();
            //});
            while (true)
            {
                BrokeredMessage msg = client.Receive(TimeSpan.FromMinutes(1));

                if (msg == null)
                {
                    Console.WriteLine("No more messages....");
                    break;
                }
                //Deserialize the message
                PizzaOrder order = msg.GetBody<PizzaOrder>();

                //Display the message properties
                Console.WriteLine($"Message Id# {msg.MessageId}");
                Console.WriteLine($"Name : {order.Name}");
                Console.WriteLine($"Size : {order.Size}");
                Console.WriteLine($"Has Meat : {order.HasMeat}");
                
                //Complete the message
                msg.Complete();

                //Thread.Sleep(TimeSpan.FromMinutes(1));
               
            }

            Console.ResetColor();
            client.Close();
            factory.Close();

            Console.WriteLine("Receive complete.Press any key to exit");
            Console.ReadKey();
        }
    }
}
