using System;
using System.Messaging;
using System.Xml.Linq;

namespace BluffCityInformationCenter
{
    class Program
    {
        static void Main(string[] args)
        {

            MessageQueue messageQueue = null;
            if (MessageQueue.Exists(@".\Private$\AirportInfoGateNo"))
            {
                messageQueue = new MessageQueue(@".\Private$\AirportInfoGateNo");
                messageQueue.Label = "AirportInfoQueue";
            }
            else
            {
                // Opret Queue
                MessageQueue.Create(@".\Private$\AirportInfoGateNo");
                messageQueue = new MessageQueue(@".\Private$\AirportInfoGateNo");
                messageQueue.Label = "AirportInfoQueue";
            }

            XElement booksFromFile = XElement.Load(@"AirportInforGateNoSAS.xml");
            Console.WriteLine(booksFromFile);
            string AirlineCompany = "SAS";

            messageQueue.Send(booksFromFile, AirlineCompany);

            booksFromFile = XElement.Load(@"AirportInforGateNoKLM.xml");
            Console.WriteLine(booksFromFile);
            AirlineCompany = "KLM";

            messageQueue.Send(booksFromFile, AirlineCompany);

            booksFromFile = XElement.Load(@"AirportInforGateNoSW.xml");
            Console.WriteLine(booksFromFile);
            AirlineCompany = "SW";

            messageQueue.Send(booksFromFile, AirlineCompany);


            while (true) { }
        }
    }
}

