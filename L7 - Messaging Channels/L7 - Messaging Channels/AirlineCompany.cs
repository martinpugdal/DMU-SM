﻿using System;

namespace L7___Messaging_Channels
{
    public class AirlineCompany
    {
        public AirlineCompany(string companyName, string departure, string flightNo, string destination, DateTime arrived_at)
        {
            CompanyName = companyName;
            Departure = departure;
            FlightNo = flightNo;
            Destination = destination;
            ArrivedAt = arrived_at;
        }

        public string CompanyName { get; set; }
        public string Departure { get; set; }
        public string FlightNo { get; set; }
        public string Destination { get; set; }
        public DateTime ArrivedAt { get; set; }

        public string ToJSON
        {
            get
            {
                return
                    "{ " +
                        @"CompanyName: """ + CompanyName + @"""" + ", " +
                        @"FlightNo: """ + FlightNo + @"""" + ", " +
                        @"Departure: """ + Departure + @"""" + ", " +
                        @"Destination: """ + Destination + @"""" + ", " +
                        @"Arrived_ETA: """ + ArrivedAt + @"""" +
                    " }";
            }
        }
    }
}
