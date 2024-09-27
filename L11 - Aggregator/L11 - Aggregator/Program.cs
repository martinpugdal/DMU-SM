using System;
using System.Messaging;

namespace L11___Aggregator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string path = @".\Private$\AirportOutput";
            MessageQueue messageQueue = new MessageQueue(path);
            if (!MessageQueue.Exists(path))
            {
                MessageQueue.Create(path);
            }

            new Aggregator(messageQueue);

            Console.ReadLine();
        }
    }
}
