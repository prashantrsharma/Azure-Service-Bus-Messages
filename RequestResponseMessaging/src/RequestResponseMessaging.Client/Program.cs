using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using RequestResponseMessaging.Configuration;

namespace RequestResponseMessaging.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Client Console";
            Console.WriteLine("Client Console.");

            //Create the Messaging Factory
            MessagingFactory factory = MessagingFactory.CreateFromConnectionString(AccountDetails.ConnectionString);

            //Create Request and Response Queue Clients
            QueueClient requestClient = factory.CreateQueueClient(AccountDetails.RequestQueueName);
            QueueClient responseClient = factory.CreateQueueClient(AccountDetails.ResponseQueueName);

            while (true)
            {
                Console.WriteLine("Enter text:");
                string text = Console.ReadLine();

                //Create a session identifier for the response message
                string responseSessionId = Guid.NewGuid().ToString("N");

                //Create a message using text as body
                BrokeredMessage msg = new BrokeredMessage(text);

                //Set the appropriate message properties
                msg.ReplyToSessionId = responseSessionId;

                //Start the Stop Watch
                Stopwatch watch = Stopwatch.StartNew();

                //Send the message on the request queue.
                requestClient.Send(msg);

                //Accept a session message
                MessageSession responseSession = responseClient.AcceptMessageSession(responseSessionId);

                //Receive the response message
                BrokeredMessage responseMsg = responseSession.Receive();
                watch.Stop();

                //Close the session, we got the message
                responseSession.Close();
                
                //Deserialize the message body
                string echoText = responseMsg.GetBody<string>();

                Console.WriteLine(echoText);
                Console.WriteLine($"Time:{watch.ElapsedMilliseconds} ms.");
                Console.WriteLine();
            }
        }
    }
}
