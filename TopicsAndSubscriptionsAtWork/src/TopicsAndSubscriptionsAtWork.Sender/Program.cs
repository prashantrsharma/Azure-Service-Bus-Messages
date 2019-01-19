using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using TopicsAndSubscriptionsAtWork.Configuration;
using TopicsAndSubscriptionsAtWork.Models;

namespace TopicsAndSubscriptionsAtWork.Sender
{
    class Program
    {
        private static readonly List<PizzaOrder> PizzaOrders = new List<PizzaOrder>();
        static void Main(string[] args)
        {
            Console.Title = "Sender-Console";
            Console.WriteLine("Sender-Console");
            Console.WriteLine("Press any key to proceed......");
            Console.ReadLine();

            //Create a Namespace Manager
            Console.WriteLine("Creating Namespace Manager....");
            NamespaceManager nsManager = NamespaceManager.CreateFromConnectionString(Settings.ConnectionString);

            //Create a Topic 
            if (nsManager.TopicExists(Settings.TopicName))
            {
                nsManager.DeleteTopic(Settings.TopicName);
            }

            TopicDescription topicDesc = new TopicDescription(Settings.TopicName)
            {
               
            };
            
            //Create a Topic
            Console.WriteLine($"Creating Topic with name : {Settings.TopicName} .......");
            nsManager.CreateTopic(topicDesc);

            SubscriptionDescription subscriptionDesc =
                new SubscriptionDescription(Settings.TopicName, Settings.PizzaSubscription)
                {
                    
                };

            //Create  a Subscription
            Console.WriteLine($"Creating Subscription {subscriptionDesc.Name} for topic with name : {Settings.TopicName} .......");
            nsManager.CreateSubscription(subscriptionDesc);
            nsManager.CreateSubscription(Settings.TopicName, Settings.PizzaSubscriptionWithMeat, new SqlFilter("HasMeat = true"));

            //Create Orders
            Console.WriteLine("Create Orders...");
            CreateOrders();

            //Send some orders to recipients
            Console.WriteLine("Send Orders....");
            SendOrders();

            Console.WriteLine("Send complete.Press any key to exit");
            Console.ReadKey();
        }

        private static void CreateOrders()
        {
           PizzaOrders.Add( new PizzaOrder
           {
               Name = "Veggie Delight",
               HasMeat = false,
               Size = "Large"
           });
           PizzaOrders.Add( new PizzaOrder
           {
               Name = "Garden Fresh",
               HasMeat = false,
               Size = "Large"
           });
           PizzaOrders.Add(new PizzaOrder
           {
               Name = "Chicken Farm Fresh",
               HasMeat = true,
               Size = "XX Large"
           });
           PizzaOrders.Add(new PizzaOrder
           {
               Name = "Papa's Bacon Special",
               HasMeat = true,
               Size = "XXX Large"
           });
        
        }

        private static void SendOrders()
        {
            //Create a Message Factory
            MessagingFactory factory = MessagingFactory.CreateFromConnectionString(Settings.ConnectionString);

            //Create a Topic Client
            TopicClient client = factory.CreateTopicClient(Settings.TopicName);

            foreach (PizzaOrder order in PizzaOrders)
            {
                BrokeredMessage msg = new BrokeredMessage(order);
                msg.Properties.Add("Name", order.Name);
                msg.Properties.Add("HasMeat",order.HasMeat);
                msg.Properties.Add("Size",order.Size);

                Console.WriteLine($"Sending message with message id#{msg.MessageId}");
                client.Send(msg);
            }
           
            //Close the Client
            client.Close();

            //Close the Messaging Factory
            factory.Close();
        }
    }
}
