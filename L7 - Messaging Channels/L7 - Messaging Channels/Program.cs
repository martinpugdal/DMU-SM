using System;
using System.Collections.Generic;
using System.Messaging;

namespace L7___Messaging_Channels
{
    internal class Program // Air Traffic Control (sends messages to AIC)
    {
        private static MessageQueue airlineInfoCenterMsgQueue;
        private static readonly Dictionary<string, MessageQueue> airlinesCompanyMsgQueue = new Dictionary<string, MessageQueue>();

        static void Main()
        {
            // create the airline information center
            string path = @".\Private$\AirlineInformationCenter";
            if (!MessageQueue.Exists(path))
                MessageQueue.Create(path);
            airlineInfoCenterMsgQueue = new MessageQueue(path)
            {
                Label = "AirlineInformationCenter"
            };

            Publisher publisher = new Publisher();


            // create the airline companies
            List<string> airlineCompanies = new List<string> {
                "SAS",
                "Norwegian",
                "Ryanair",
                "KLM"
            };

            // iterate through the airline companies and setup the message queues
            for (int i = 0; i < airlineCompanies.Count; i++)
            {
                string airlineCompany = airlineCompanies[i];
                string airlineCompanyPath = @".\Private$\" + airlineCompany;
                if (!MessageQueue.Exists(airlineCompanyPath))
                    MessageQueue.Create(airlineCompanyPath);
                MessageQueue airlineCompanyMsgQueue = new MessageQueue(airlineCompanyPath)
                {
                    Label = airlineCompany
                };
                airlinesCompanyMsgQueue.Add(airlineCompany, airlineCompanyMsgQueue);
                publisher.Subscribe(airlineCompany);
            }

            // create the router
            new AirlineCenterRouter(airlineInfoCenterMsgQueue, airlinesCompanyMsgQueue, publisher);

            // send the airline company message to the airline information center
            AirlineCompany airlineCompany1 = new AirlineCompanyBuilder().
                WithCompanyName("SAS").
                WithFlightNo("SK123").
                WithDestination("Billund").
                WithDeparture("10:10PM").
                withArrivedETA(DateTime.Now.AddMinutes(10)).
                Build();

            // parsed to json and sent to the airline information center
            //string json = JsonConvert.SerializeObject(airlineCompany1);
            string json = airlineCompany1.ToJSON;
            //Console.WriteLine(json);
            airlineInfoCenterMsgQueue.Send(json, airlineCompany1.CompanyName);

            // keep alive
            Console.ReadLine();
        }
    }
}
