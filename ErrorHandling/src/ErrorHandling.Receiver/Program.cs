using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ErrorHandling.Config;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace ErrorHandling.Receiver
{
    class Program
    {
        public static string SubscriptionName;

        static void Main(string[] args)
        {
            Console.Title = "Receiver Console";
            Console.WriteLine("Receiver Console");

            Utils.WriteLine("Enter Subscription name",ConsoleColor.Yellow);
            SubscriptionName = Console.ReadLine();

            try
            {
                CreateSubscription(SubscriptionName);
                Utils.WriteLine("Press enter to receive messages", ConsoleColor.Yellow);
                Console.ReadLine();
                ReceiveMessages();
            }
            catch (Exception e)
            {
               Utils.WriteLine(e.Message,ConsoleColor.Red);
            }

            Console.ReadLine();
        }

        private static void CreateSubscription(string subscriptionName)
        {
            //Create Namespace Manager
            NamespaceManager nsManager = NamespaceManager.CreateFromConnectionString(ConfigurationSettings.ConnectionString);

            //Define Subscription Description
            SubscriptionDescription description =
                new SubscriptionDescription(ConfigurationSettings.TopicName, ConfigurationSettings.QueueName)
                {
                    LockDuration = TimeSpan.FromSeconds(10),
                    AutoDeleteOnIdle = TimeSpan.FromMinutes(5),
                    DefaultMessageTimeToLive = TimeSpan.FromMinutes(30),
                    EnableDeadLetteringOnMessageExpiration = true
                };

            //Create Subscription
            nsManager.CreateSubscription(description);
        }

        private static void ReceiveMessages()
        {
            //Create Message Factory
            MessagingFactory factory = MessagingFactory.CreateFromConnectionString(ConfigurationSettings.ConnectionString);
            Utils.WriteLine("Creating Messaging Factory", ConsoleColor.Cyan);

            //Create a Queue Client
            QueueClient subscriptionClient = factory.CreateQueueClient(ConfigurationSettings.ConnectionString);
            Utils.WriteLine("Creating Subscription Client", ConsoleColor.Cyan);

            OnMessageOptions msgOptions = new OnMessageOptions()
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false,
                AutoRenewTimeout =  TimeSpan.FromSeconds(1)
            };

            subscriptionClient.OnMessage(ProcessMessage,msgOptions);

        }

        private static void ProcessMessage(BrokeredMessage message)
        {
            Utils.WriteLine($"Received : {message.Label}",ConsoleColor.Cyan);
            switch (message.ContentType)
            {
                case "text/plain":
                    ProcessTextMessage(message);
                    break;
                case "application/json":
                    ProcessTextMessage(message);
                    break;
                default:
                    Utils.WriteLine($"Received unknown message {message.ContentType}",ConsoleColor.Red);
                    //message.Abandon();
                    message.DeadLetter($"Unknown message type",$"The message type :{message.ContentType} is not unknown");
                    break;
            }
        }

        private static void ProcessTextMessage(BrokeredMessage msg)
        {
            Utils.WriteLine($"Text message:{msg.Label}",ConsoleColor.Cyan);

            //Clone and Forward the message to queue
            try
            {
                //BrokeredMessage forwardedMsg = msg.Clone();
                //QueueClient queueClient = QueueClient.CreateFromConnectionString(ConfigurationSettings.ConnectionString);
                //queueClient.Send(forwardedMsg);
                //queueClient.Close();
                msg.Complete();
                Utils.WriteLine("Processed msg", ConsoleColor.Cyan);
            }
            catch (MessagingException mex)
            {
                Utils.WriteLine($"Exception {mex.Message}", ConsoleColor.Red);
                Utils.WriteLine($"The fault {(mex.IsTransient ? "is" : "is not")} transient.", ConsoleColor.DarkYellow);

                if (msg.DeliveryCount > 3)
                {
                    msg.DeadLetter(mex.Message, mex.ToString());
                }
                else
                {
                    //Abandon the message
                    //msg.Abandon();
                }
            }
        }

        private static void ProcessJsonMessage(BrokeredMessage msg)
        {
            Utils.WriteLine("JSON message: " + msg.Label, ConsoleColor.Yellow);

            try
            {
                //var storageAccount = CloudStorageAccount.Parse(Settings.BadStorageAccountConnectionString);
                //var blobContainer = storageAccount.CreateCloudBlobClient().GetContainerReference("blobs");
                //var blockBlob = blobContainer.GetBlockBlobReference(message.MessageId);
                //blockBlob.UploadText(message.Label);
                msg.Complete();
                Utils.WriteLine("Processed message", ConsoleColor.Cyan);
            }
            catch (Exception ex)
            {
                if (msg.DeliveryCount > 3)
                {
                    Utils.WriteLine("Dead-lettering message", ConsoleColor.Cyan);
                    msg.DeadLetter(ex.Message, ex.ToString());
                }
                else
                {
                    Utils.WriteLine("Abandoning message", ConsoleColor.Cyan);
                    msg.Abandon();
                }

            }

        }
    }
}
