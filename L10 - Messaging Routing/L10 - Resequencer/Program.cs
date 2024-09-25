using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace L10___Resequencer
{
    internal class Program
    {
        static void Main(string[] args)
        {

            string pathPassenger = @".\Private$\AirportCheckInInput";
            string pathLuggage = @".\Private$\AirportLuggageInput";
            string pathOutput = @".\Private$\AirportOutput";
            MessageQueue passengerQueue = new MessageQueue(pathPassenger);
            MessageQueue luggageQueue = new MessageQueue(pathLuggage);
            MessageQueue outputQueue = new MessageQueue(pathOutput);
            if (!MessageQueue.Exists(pathPassenger))
            {   
                MessageQueue.Create(pathPassenger);
            }
            if (!MessageQueue.Exists(pathLuggage))
            {
                MessageQueue.Create(pathLuggage);
            }
            if (!MessageQueue.Exists(pathOutput))
            {
                MessageQueue.Create(pathOutput);
            }


            new Reqsequencer(outputQueue, passengerQueue, luggageQueue);

            Console.ReadLine();
        }
    }
}
