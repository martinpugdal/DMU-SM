using System;
using System.Messaging;

namespace L10___Messaging_Routing
{
    internal class Program
    {
        static void Main(string[] args)
        {

            string path = @".\Private$\AirportCheckInOutput";
            MessageQueue messageQueue = new MessageQueue(path);
            if (!MessageQueue.Exists(path))
            {
                MessageQueue.Create(path);
            }

            new Splitter(messageQueue);

            Console.ReadLine();
        }
    }
}
