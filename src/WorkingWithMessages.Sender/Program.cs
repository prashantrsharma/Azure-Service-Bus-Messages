using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using WorkingWithMessages.Config;

namespace WorkingWithMessages.Sender
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Sender Console - Hit Enter");
            Console.ReadLine();
            Console.WriteLine("Sender Console - Is ready to send new messages.....");

            Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();
            //for (int i = 0; i < 10; i++)
            //{
            //Send Text String
            SendTextString(Settings.TextString, false);
            //}

            watch.Stop();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Sender Console - Complete");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($".........It took {watch.Elapsed.TotalMinutes} minutes to process the current transaction.........");
            Console.ReadKey();

        }

        static void SendTextString(string text, bool sendSync)
        {
            //Create a Client
            QueueClient queueClient = QueueClient.CreateFromConnectionString(Settings.ConnectionString, Settings.QueueName);
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Sending:");
            foreach (char letter in text.ToCharArray())
            {
                //Create an Empty Message and Set the Label
                BrokeredMessage message = new BrokeredMessage
                {
                    Label = letter.ToString()
                };

                if (sendSync)
                {
                    //Send the Message
                    queueClient.Send(message);
                    Console.WriteLine(message.Label);
                }
                else
                {
                    Task.Run(async () =>
                    {
                        await queueClient.SendAsync(message);
                        Console.WriteLine(message.Label);
                    });
                }
            }

            // Console.ReadLine();
            Console.WriteLine();

            //Always close the client
            queueClient.Close();
        }
    }
}
