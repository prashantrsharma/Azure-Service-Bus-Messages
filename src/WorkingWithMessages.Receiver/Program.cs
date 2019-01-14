using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using WorkingWithMessages.Config;

namespace WorkingWithMessages.Receiver
{
    class Program
    {
        static readonly QueueClient Client = QueueClient.CreateFromConnectionString(Settings.ConnectionString,Settings.QueueName);
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Receiver Console - Hit Enter");
            Console.ReadLine();
            Console.WriteLine("Receiver Console - Is ready to receive new messages.....");

            //Receive and Process the message
            ReceiveAndProcess();

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
            },new OnMessageOptions{AutoComplete = true});
        }
    }
}
