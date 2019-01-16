using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using RfidCheckout.Config;
using RfidCheckout.DataContracts;

namespace RfidCheckout.Checkout
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Rfid Checkout - Receiver Application !";
            Console.WriteLine("Rfid Checkout - Receiver Application !");
            //Console.ReadLine();

            //Create the Namespace Manager
            NamespaceManager manager = NamespaceManager.CreateFromConnectionString(AccountDetails.ConnectionString);

            //Create an instance of Messaging Factory
            MessagingFactory factory = MessagingFactory.CreateFromConnectionString(AccountDetails.ConnectionString);

            //Delete the queue if its exists
            if (manager.QueueExists(AccountDetails.QueueName))
            {
                manager.DeleteQueue(AccountDetails.QueueName);
            }

            //Create a description for the Queue
            QueueDescription description = new QueueDescription(AccountDetails.QueueName)
            {
                //Comment in to require duplicate detection
                RequiresDuplicateDetection =  true,
                DuplicateDetectionHistoryTimeWindow = TimeSpan.FromMinutes(10),

                //Comment into require sessions
                RequiresSession = true
            };

            //Create a queue based on the queue description
            manager.CreateQueue(description);

            //Use the Messaging Factory to create a queue client for the specified queue
            QueueClient client = factory.CreateQueueClient(AccountDetails.QueueName);

            Console.WriteLine("Receiving tag read messages.....");

            while (true)
            {
                int receivedCount = 0;
                double billTotal = 0.0;

                //Comment in to use a session receiver
                Console.ForegroundColor = ConsoleColor.Cyan;
                MessageSession messageSession = client.AcceptMessageSession();
                Console.WriteLine($"Accepted session:{messageSession.SessionId}");

                Console.ForegroundColor = ConsoleColor.Yellow;

                while (true)
                {
                    //Receive a tag read message
                    //Swap comments to use a session receiver
                    //BrokeredMessage receivedTagRead = client.Receive(TimeSpan.FromSeconds(5));
                    BrokeredMessage receivedTagRead = messageSession.Receive(TimeSpan.FromSeconds(5));

                    if (receivedTagRead != null)
                    {
                        //Process the message
                        RfidTag tag = receivedTagRead.GetBody<RfidTag>();
                        Console.WriteLine($"Bill for {tag.Product} # ${tag.Price}");
                        receivedCount++;
                        billTotal += tag.Price;

                        //Mark the message as complete
                        receivedTagRead.Complete();

                    }
                    else
                    {
                        break;
                    }

                }

                if (receivedCount > 0)
                {
                    //Bill the Customer
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Bill Customer ${billTotal} for items {receivedCount}");
                    Console.WriteLine();
                    Console.ResetColor();
                }

            }
        }
    }
}
