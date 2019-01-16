using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using RfidCheckout.Config;
using RfidCheckout.DataContracts;

namespace RfidCheckout.TagReader
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Tag Reader - Sender Application !";
            Console.WriteLine("Tag Reader - Sender Application !");
           // Console.ReadLine();

            //Create Queue for the first time
           // CreateQueueIfNotExists();

            //Create the MessageFactory
            MessagingFactory factory = MessagingFactory.CreateFromConnectionString(AccountDetails.ConnectionString);

            //Create a Queue Client
            QueueClient client = factory.CreateQueueClient(AccountDetails.QueueName);

            //Create an Sample Order
            RfidTag[] orderItems = new RfidTag[]
            {
                new RfidTag() { Product = "Ball", Price = 4.99 },
                new RfidTag() { Product = "Whistle", Price = 1.95 },
                new RfidTag() { Product = "Bat", Price = 12.99 },
                new RfidTag() { Product = "Bat", Price = 12.99 },
                new RfidTag() { Product = "Gloves", Price = 7.99 },
                new RfidTag() { Product = "Gloves", Price = 7.99 },
                new RfidTag() { Product = "Cap", Price = 9.99 },
                new RfidTag() { Product = "Cap", Price = 9.99 },
                new RfidTag() { Product = "Shirt", Price = 14.99 },
                new RfidTag() { Product = "Shirt", Price = 14.99 },
            };

            //Display the Order Data
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\t Order contains {orderItems.Length} items.");
            Console.ForegroundColor = ConsoleColor.Yellow;

            double orderTotal = 0.0;

            foreach (RfidTag item in orderItems)
            {
                Console.WriteLine($"{item.Product}- ${item.Price}");
                orderTotal += item.Price;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Order value = $ {orderTotal}");
            Console.WriteLine();
            Console.ResetColor();

            Console.WriteLine("Press enter to scan..........");
            Console.ReadLine();

            //Send the order with Random Duplicate Tag Reads
            int sentCount = 0;
            int position = 0;
            Random random = new Random(DateTime.Now.Millisecond);

            Console.WriteLine("Reading tags.......");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;

            string sessionId = Guid.NewGuid().ToString("N");

            while (position < 10)
            {
                RfidTag rfidTag = orderItems[position];

                //Create a new brokered message from the order item Rfid Tag
                BrokeredMessage message = new BrokeredMessage(rfidTag);

                //Comment in to set message id
                message.MessageId = rfidTag.TagId;

                //Comment in to set session id
                message.SessionId = sessionId;

                //Send the message
                client.Send(message);

                Console.WriteLine($"Sent : {rfidTag.Product} - Message Id# {rfidTag.TagId}");

                //Randomly cause a duplicate message to be sent.
                if (random.NextDouble() > 0.4)
                {
                    position++;
                }
                sentCount++;

                Thread.Sleep(100);
                
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{sentCount} total tag reads..");
            Console.WriteLine();
            Console.ResetColor();
            Console.ReadLine();
        }

        static void CreateQueueIfNotExists()
        {
            NamespaceManager manager = NamespaceManager.CreateFromConnectionString(AccountDetails.ConnectionString);
            if (!manager.QueueExists(AccountDetails.QueueName))
            {
                manager.CreateQueue(AccountDetails.QueueName);
            }
           
        }
    }
}
