using System;

namespace L7___Messaging_Channels
{
    public class AirlineCompanyBuilder
    {
        private string companyName = "";
        private string departure = "";
        private string flightNo = "";
        private string destination = "";
        private DateTime eta_arrived = DateTime.MinValue;

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

        public AirlineCompanyBuilder withArrivedETA(DateTime eta)
        {
            this.eta_arrived = eta;
            return this;
        }

        public AirlineCompany Build()
        {
            return new AirlineCompany(companyName, departure, flightNo, destination, eta_arrived);
        }
    }
}
