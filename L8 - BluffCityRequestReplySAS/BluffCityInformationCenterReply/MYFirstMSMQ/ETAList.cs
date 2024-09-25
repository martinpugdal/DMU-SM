using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Messaging;
using System.Reflection.Emit;

namespace BluffCityInformationCenterETA
{
    class ETAList
    {
        class Flight
        {
            public string FlightNo { get; set; }
            public string ETA { get; set; }

        }
        List<Flight> flightList = new List<Flight>(3);

        protected string flightNo;
        protected string flightNoReturn;
        public ETAList(String FlightNo)
        {
            Flight flight1 = new Flight()
            {
                FlightNo = "SK942",
                ETA = "12:45"
            };
            Flight flight2 = new Flight()
            {
                FlightNo = "SW467",
                ETA = "13:20"
            };
            Flight flight3 = new Flight()
            {
                FlightNo = "KL105",
                ETA = "14:35",
            };

            //List    
            flightList.Add(flight1);
            flightList.Add(flight2);
            flightList.Add(flight3);

        }
        public string ReturnETA(String FlightNo)
        {
            this.flightNo = FlightNo;

            flightNoReturn = null;
            foreach (Flight f in flightList)
            {
                if (f.FlightNo == this.flightNo)
                            flightNoReturn = f.ETA;
            }
            return flightNoReturn;
        }
    }
}
