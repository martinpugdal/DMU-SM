using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Messaging;

namespace L2___Messaging_System_og_Channels
{
    internal class Program
    {
        private static MessageQueue airlineInfoCenterMsgQueue;
        private static Dictionary<string, MessageQueue> airlinesCompanyMsgQueue = new Dictionary<string, MessageQueue>();

        static void Main(string[] args)
        {
            // create the airline information center
            string path = @".\Private$\AirlineInformationCenter";
            if (!MessageQueue.Exists(path))
                MessageQueue.Create(path);
            airlineInfoCenterMsgQueue = new MessageQueue(path)
            {
                Label = "AirlineInformationCenter"
            };

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
            }

            // create the router
            new AirlineCenterRouter(airlineInfoCenterMsgQueue, airlinesCompanyMsgQueue);

            // send the airline company message to the airline information center
            AirlineCompanyBuilder jsonAirlineCompanyBuilder = new AirlineCompanyBuilder();
            AirlineCompany airlineCompany1 = jsonAirlineCompanyBuilder
                .WithCompanyName("TEST")
                .WithDeparture("10:00PM")
                .WithFlightNo("SK123")
                .WithDestination("JFK")
                .WithCheckIn("08:00PM")
                .WithGate("G-2")
                .Build();

            // parsed to json and sent to the airline information center
            string json = JsonConvert.SerializeObject(airlineCompany1);
            airlineInfoCenterMsgQueue.Send(json, airlineCompany1.CompanyName);

            // keep alive
            Console.ReadLine();
        }
    }
}
