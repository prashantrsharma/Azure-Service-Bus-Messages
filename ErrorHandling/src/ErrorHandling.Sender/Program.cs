using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using ErrorHandling.Config;
using ConfigurationSettings = ErrorHandling.Config.ConfigurationSettings;

namespace ErrorHandling.Sender
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Sender Console";
            Console.WriteLine(QueueClient.FormatDeadLetterPath(ConfigurationSettings.QueueName));
            Console.WriteLine(SubscriptionClient.FormatDeadLetterPath(ConfigurationSettings.TopicName,ConfigurationSettings.QueueName));



            //Console.WriteLine("Press enter to send a JSON message");
            //Console.ReadLine();
            //SendMessage("{\"contact\": {\"name\": \"Alan\",\"twitter\": \"@alansmith\" }}", "application/json");

            //Console.WriteLine("Press enter to send a poison message");
            //Console.ReadLine();
            //SendMessage("I got the poison", "potion/poison");


        }
    }
}
