using System;
using System.Text;
using System.Linq;
using System.Xml.Linq;
using System.Messaging;
using System.Text.RegularExpressions;
using System.Text.Json;

namespace BluffCityInformationCenter
{

    public class AirportInfoGateNo
    {
        public String Airline_Id { get; set; }
        public String Flight_No { get; set; }
        public String Destination { get; set; }
        public String Time_Departure { get; set; }
        public String Checkin_Time { get; set; }
        public String Gate_No { get; set; }
    }
    public class Program
    {
        public static void Main(string[] args)
        {
            var SASairportInfoGateNo = new AirportInfoGateNo
            {
                Airline_Id = "SAS",
                Flight_No = "SK952",
                Destination = "Amsterdam Schipol (AMS)",
                Time_Departure = "12:10",
                Checkin_Time = "11:45",
                Gate_No = "B12"
            };

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

             string SASjsonString = JsonSerializer.Serialize(SASairportInfoGateNo);

             string AirlineCompany = "SAS";

             messageQueue.Send(SASjsonString, AirlineCompany);

            var KLMairportInfoGateNo = new AirportInfoGateNo
            {
                Airline_Id = "KLM",
                Flight_No = "KL1264",
                Destination = "Amsterdam Schipol (AMS)",
                Time_Departure = "12:10",
                Checkin_Time = "11:45",
                Gate_No = "B15"
            };
             
            AirlineCompany = "KLM";
            string KLMjsonString = JsonSerializer.Serialize(KLMairportInfoGateNo);

            messageQueue.Send(KLMjsonString, AirlineCompany); ;

            var SWairportInfoGateNo = new AirportInfoGateNo
            {
                Airline_Id = "SW",
                Flight_No = "SW524",
                Destination = "Amsterdam Schipol (AMS)",
                Time_Departure = "12:10",
                Checkin_Time = "11:45",
                Gate_No = "A17"
            };

            string SWjsonString = JsonSerializer.Serialize(SWairportInfoGateNo);

            AirlineCompany = "SW";

            messageQueue.Send(SWjsonString, AirlineCompany);

        }
    }
}

