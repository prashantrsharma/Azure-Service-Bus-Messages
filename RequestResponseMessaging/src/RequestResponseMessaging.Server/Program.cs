using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using RequestResponseMessaging.Configuration;

namespace RequestResponseMessaging.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Server Console";
            Console.WriteLine("Server Console");

            //Create the NamespaceManager
            NamespaceManager sbManager = NamespaceManager.CreateFromConnectionString(AccountDetails.ConnectionString);

            //Create the Messaging Factory
            MessagingFactory sbFactory = MessagingFactory.CreateFromConnectionString(AccountDetails.ConnectionString);

            Console.WriteLine("Create Queues.......");

            //Delete any existing queues
            if (sbManager.QueueExists(AccountDetails.RequestQueueName))
            {
                sbManager.DeleteQueue(AccountDetails.RequestQueueName);
            }

            if (sbManager.QueueExists(AccountDetails.ResponseQueueName))
            {
                sbManager.DeleteQueue(AccountDetails.ResponseQueueName);
            }

            //Create Request Queue
            sbManager.CreateQueue(AccountDetails.RequestQueueName);

            //Create a response queue with sessions
            QueueDescription responseQueueDescription = new QueueDescription(AccountDetails.ResponseQueueName)
            {
                RequiresSession = true
            };

            //Create Response Queue
            sbManager.CreateQueue(responseQueueDescription);

            Console.WriteLine("Done!");

            //Create Request and Response Queue Clients
            QueueClient requestQueueClient = sbFactory.CreateQueueClient(AccountDetails.RequestQueueName);
            QueueClient responseQueueClient = sbFactory.CreateQueueClient(AccountDetails.ResponseQueueName);

            requestQueueClient.OnMessage(message =>
            {
                //Deserialize the message body into text
                string text = message.GetBody<string>();
                Console.WriteLine($"Received:{text}");
                Thread.Sleep(DateTime.Now.Millisecond*20);

                string echoText = "Echo: " + text;

                //Create a response text using echoText as the message body
                BrokeredMessage responseMsg = new BrokeredMessage(echoText);

                //Set the session id
                responseMsg.SessionId = message.ReplyToSessionId;

                //Send the message
                responseQueueClient.Send(responseMsg);
                Console.WriteLine($"Sent : {echoText}");
            } , new OnMessageOptions() {AutoComplete = true,MaxConcurrentCalls = 10});

            Console.WriteLine("Processing, hit enter to exit");
            Console.ReadLine();
        }
    }
}
