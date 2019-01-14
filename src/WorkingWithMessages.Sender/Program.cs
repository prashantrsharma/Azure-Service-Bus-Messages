﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using WorkingWithMessages.Config;
using WorkingWithMessages.DataContracts;

namespace WorkingWithMessages.Sender
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Sender";
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Sender Console - Hit Enter");
            Console.ReadLine();
            Console.WriteLine("Sender Console - Is ready to send new messages.....");

            //Start the Stopwatch
            Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();

            //Send Text String
            //SendTextString(Settings.TextString, false);

            //Send Control message
            //SendControlMessage();

            //Send Pizza Order
            SendPizzaOrder();

            //Stop the Stopwatch
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
                    Task.Run(() =>
                    {
                        queueClient.SendAsync(message);
                        Console.WriteLine(message.Label);
                    });
                }
            }

            // Console.ReadLine();
            Console.WriteLine();

            //Always close the client
            queueClient.Close();
        }

        static void SendControlMessage()
        {
            //Create a message with no body

            BrokeredMessage message = new BrokeredMessage
            {
                Label = "Control"
            };

            //Add some properties to the property collection
            message.Properties.Add("SystemId",1462);
            message.Properties.Add("Command","Pending Restart");
            message.Properties.Add("ActionTime",DateTime.UtcNow.AddHours(2));

            //Send the message
            QueueClient client = QueueClient.CreateFromConnectionString(Settings.ConnectionString,Settings.QueueName);
            Console.WriteLine("Sending control message....");
            client.Send(message);
            Console.WriteLine("Done!");

            Console.WriteLine("Send Again?");
            string response = Console.ReadLine();

            if (response.ToLower() == "y")
            {
                //Try to send the message a second time.
                Console.WriteLine("Sending control message again.....");
                message = message.Clone();
                client.Send(message);
                Console.WriteLine("Done!");
            }

            //Always close the client
            client.Close();

        }

        static void SendPizzaOrder()
        {
            PizzaOrder order = new PizzaOrder
            {
                CustomerName = "Allan Smith",
                Type = "Hawaiian",
                Size = "Large"
            };

            //Create a Brokered Message
            BrokeredMessage message = new BrokeredMessage(order)
            {
                Label = "PizzaOrder"
            };

            //What is the size of the message?
            Console.WriteLine($"Message size : {message.Size}");

            //Send the message
            QueueClient client = QueueClient.CreateFromConnectionString(Settings.ConnectionString, Settings.QueueName);
            Console.WriteLine("Sending order.........");
            client.Send(message);
            Console.WriteLine("Done!");

            //What is the message now?
            Console.WriteLine($"Message size : {message.Size}");
        }
    }
}
