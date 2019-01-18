using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Configuration;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using TopicsAndSubscriptions.Models;

namespace TopicsAndSubscriptions
{
    class Program
    {
        private const string TopicPath = "orderTopic";
        static NamespaceManager _manager;
        static MessagingFactory _factory;
        static TopicClient _orderTopicClient;
        private const string AllOrdersSubscription = "allOrdersSubscription";

        static void Main(string[] args)
        {
            CreateManagerAndFactory();
            Console.ForegroundColor = ConsoleColor.Cyan;

            Console.WriteLine("Creating Topics and Subscriptions");
            CreateTopicsAndSubscriptions();
            Console.WriteLine("Done!");

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Press enter to send messages");
            Console.ReadLine();

            //Create a Topic Client for ordertopic
            _orderTopicClient = _factory.CreateTopicClient(TopicPath);

            Console.WriteLine("Sending orders.....");

            // Send five orders with different properties.
            SendOrder();

            //Close the Topic Client
            _orderTopicClient.Close();

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Press enter to receive messages.");
            Console.ReadLine();

            //Receive all messages from the OrderTopic Subscription 
            ReceiveFromSubscription(TopicPath);

            //Close the MessagingFactory and all it created
            _factory.Close();

            Console.ReadKey();
        }

        private static void SendOrder()
        {
            SendOrder(new Order()
            {
                Name = "Loyal Customer",
                Value = 19.99,
                Region = "USA",
                Items = 1,
                HasLoyaltyCard = true
            });

            SendOrder(new Order()
            {
                Name = "Large Order",
                Value = 49.99,
                Region = "USA",
                Items = 50,
                HasLoyaltyCard = false
            });

            SendOrder(new Order()
            {
                Name = "High Value Order",
                Value = 749.45,
                Region = "USA",
                Items = 45,
                HasLoyaltyCard = false
            });

            SendOrder(new Order()
            {
                Name = "Loyal Europe Order",
                Value = 49.45,
                Region = "EU",
                Items = 3,
                HasLoyaltyCard = true
            });

            SendOrder(new Order()
            {
                Name = "UK Order",
                Value = 49.45,
                Region = "UK",
                Items = 3,
                HasLoyaltyCard = false
            });
        }

        private static void ReceiveFromSubscription(string topicPath)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"Receiving from topic : {TopicPath} subscriptions");

            //Loop through the subscriptions in an topic
            foreach (SubscriptionDescription subscriptionDescription in _manager.GetSubscriptions(TopicPath))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"\t Receiving from subscription \t {subscriptionDescription.Name}.....");

                //Create and SubscriptionClient
                SubscriptionClient client = _factory.CreateSubscriptionClient(TopicPath, subscriptionDescription.Name);

                //Receive all the messages from the subscription
                Console.ForegroundColor = ConsoleColor.Green;
                while (true)
                {
                    //Receive any message with a one second timeout
                    BrokeredMessage message = client.Receive(TimeSpan.FromMinutes(5));
                    if (message != null)
                    {
                        //Deserialize the message body to an order
                        Order order = message.GetBody<Order>();
                        Console.WriteLine($"Name {order.Name} {order.Region} items {order.Items}  ${order.Value} {order.HasLoyaltyCard}");

                        //Mark the message as complete
                        message.Complete();
                    }
                    else
                    {
                        Console.WriteLine();
                        break;
                    }

                }
                //Close the subscription client
                client.Close();
            }
            Console.ResetColor();
        }

        private static void SendOrder(Order order)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"Sending {order.Name}.....");

            //Create a message from the order
            BrokeredMessage orderMsg = new BrokeredMessage(order);

            //Promote Properties
            orderMsg.Properties.Add("HasLoyaltyCard", order.HasLoyaltyCard);
            orderMsg.Properties.Add("Items", order.Items);
            orderMsg.Properties.Add("Value", order.Value);
            orderMsg.Properties.Add("Region", order.Region);

            //Set the Correlation Id to the region
            orderMsg.CorrelationId = order.Region;

            //Send the message 
            _orderTopicClient.Send(orderMsg);

            Console.WriteLine("Done!");
        }

        private static void CreateTopicsAndSubscriptions()
        {
            //If Topic exists, delete it 
            if (_manager.TopicExists(TopicPath))
            {
                _manager.DeleteTopic(TopicPath);
            }

            //Create the Topic
            _manager.CreateTopic(TopicPath);

            //Subscription for all orders
            _manager.CreateSubscription(TopicPath, AllOrdersSubscription);

            //Subscriptions for USA and EU regions
            _manager.CreateSubscription(TopicPath, "usaSubscription", new SqlFilter("Region = 'USA'"));
            _manager.CreateSubscription(TopicPath, "euSubscription", new SqlFilter("Region = 'EU'"));
            _manager.CreateSubscription(TopicPath, "euSubscription2", new SqlFilter("Region = 'eu'"));

            //Subscriptions for large orders, high value orders and loyal USA customers
            _manager.CreateSubscription(TopicPath, "largeOrderSubscription", new SqlFilter("Items > 30"));
            _manager.CreateSubscription(TopicPath, "highValueSubscription", new SqlFilter("Value > 500"));
            _manager.CreateSubscription(TopicPath, "loyaltySubscription", new SqlFilter("HasLoyaltyCard = true AND Region = 'USA'"));

            //Correlation Filter for UK Customers
            _manager.CreateSubscription(TopicPath, "ukSubscription", new CorrelationFilter("UK"));
        }

        private static void CreateManagerAndFactory()
        {
            //Retrieve the Connection String
            string connectionString = 
                ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];

            //Create the Namespace Manager
            _manager =
                NamespaceManager.CreateFromConnectionString(connectionString);

            //Create the Factory
            _factory = MessagingFactory.CreateFromConnectionString(connectionString);
        }
    }
}
