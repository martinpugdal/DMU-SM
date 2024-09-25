using System;
using System.Text;
using System.Linq;
using System.Xml.Linq;
using System.Messaging;

//{
    class Program
    {
        static void Main(string[] args)
        {

            MessageQueue messageQueue = null;

            messageQueue = new MessageQueue(@".\Private$\AirportInfoGateNo");
            messageQueue.Label = "GateInfo Queue";

            //Multicast Queue
            MessageQueue multicastQueue = new MessageQueue("FormatName:MULTICAST=234.1.1.1:8001");

            AirlineCompanyPublisher airlineCompanyPublisher = null;

            
            airlineCompanyPublisher = new AirlineCompanyPublisher(messageQueue, multicastQueue);

            while (true) { } 
        }
    }
//}

