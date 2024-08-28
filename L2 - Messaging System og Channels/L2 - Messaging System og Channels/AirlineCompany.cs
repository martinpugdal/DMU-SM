namespace L2___Messaging_System_og_Channels
{
    public class AirlineCompany
    {
        public AirlineCompany(string companyName, string departure, string flightNo, string destination, string checkIn, string gate)
        {
            CompanyName = companyName;
            Departure = departure;
            FlightNo = flightNo;
            Destination = destination;
            CheckIn = checkIn;
            Gate = gate;
        }

        public string CompanyName { get; set; }
        public string Departure { get; set; }
        public string FlightNo { get; set; }
        public string Destination { get; set; }
        public string CheckIn { get; set; }
        public string Gate { get; set; }
    }
}
