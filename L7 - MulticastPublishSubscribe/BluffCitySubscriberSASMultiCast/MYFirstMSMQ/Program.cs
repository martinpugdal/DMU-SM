using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Xml.Linq;
using System.Messaging;
using System.IO;

namespace MYFirstMSMQ
{
    class Program
    {
        static void Main(string[] args)
        {

            MessageQueue messageQueueSAS = null;

            messageQueueSAS = new MessageQueue(@".\Private$\AirportCompanySASMultiCast");
            messageQueueSAS.Label = "SAS Queue";

            AirlineCompanySubscriber airlineCompanySubscriber = null;

            airlineCompanySubscriber = new AirlineCompanySubscriber(messageQueueSAS);
            while (true) { } 
        }
    }
}

