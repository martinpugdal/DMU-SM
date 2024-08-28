namespace L2___Messaging_System_og_Channels
{
    public class AirlineCompanyBuilder
    {
        private string companyName;
        private string departure;
        private string flightNo;
        private string destination;
        private string checkIn;
        private string gate;

        public AirlineCompanyBuilder WithCompanyName(string companyName)
        {
            this.companyName = companyName;
            return this;
        }

        public AirlineCompanyBuilder WithDeparture(string departure)
        {
            this.departure = departure;
            return this;
        }

        public AirlineCompanyBuilder WithFlightNo(string flightNo)
        {
            this.flightNo = flightNo;
            return this;
        }

        public AirlineCompanyBuilder WithDestination(string destination)
        {
            this.destination = destination;
            return this;
        }

        public AirlineCompanyBuilder WithCheckIn(string checkIn)
        {
            this.checkIn = checkIn;
            return this;
        }

        public AirlineCompanyBuilder WithGate(string gate)
        {
            this.gate = gate;
            return this;
        }

        public AirlineCompany Build()
        {
            return new AirlineCompany(companyName, departure, flightNo, destination, checkIn, gate);
        }
    }
}
