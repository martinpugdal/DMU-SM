using System;
using System.Text;
using System.Linq;
using System.Xml.Linq;
using System.Messaging;
using System.Text.RegularExpressions;
using System.Text.Json;

namespace BluffCityInformationCenterETA
{

    public class AirportInfoETA
    {
        public String Flight_No { get; set; }
        public String ETA { get; set; }
    }
    public class Program
    {
        public static void Main(string[] args)
        {
            var SASairportInfoETA = new AirportInfoETA
            {
                Flight_No = "SK952",
                ETA = "12:10",
            };

            MessageQueue requestETAQueue = null;
            if (MessageQueue.Exists(@".\Private$\RequestETAQueue"))
            {
                requestETAQueue = new MessageQueue(@".\Private$\RequestETAQueue");
                requestETAQueue.Label = "Request ETA Queue";
            }
            else
            {
                // Create the Queue
                MessageQueue.Create(@".\Private$\RequestETAQueue");
                requestETAQueue = new MessageQueue(@".\Private$\RequestETAQueue");
                requestETAQueue.Label = "Request ETA Queue";
            }

            MessageQueue messageQueueSAS = null;
            if (MessageQueue.Exists(@".\Private$\AirportCompanySAS"))
            {
                messageQueueSAS = new MessageQueue(@".\Private$\AirportCompanySAS");
                messageQueueSAS.Label = "SAS Queue";
            }
            else
            {
                // Create the Queue
                MessageQueue.Create(@".\Private$\AirportCompanySAS");
                messageQueueSAS = new MessageQueue(@".\Private$\AirportCompanySAS");
                messageQueueSAS.Label = "SAS Queue";
            }


            MessageQueue messageQueueKLM = null;
            if (MessageQueue.Exists(@".\Private$\AirportCompanyKLM"))
            {
                messageQueueKLM = new MessageQueue(@".\Private$\AirportCompanyKLM");
                messageQueueKLM.Label = "KLM Queue";
            }
            else
            {
                // Create the Queue
                MessageQueue.Create(@".\Private$\AirportCompanyKLM");
                messageQueueKLM = new MessageQueue(@".\Private$\AirportCompanyKLM");
                messageQueueKLM.Label = "KLM Queue";
            }

            MessageQueue messageQueueSW = null;
            if (MessageQueue.Exists(@".\Private$\AirportCompanySW"))
            {
                messageQueueSW = new MessageQueue(@".\Private$\AirportCompanySW");
                messageQueueSW.Label = "South West Queue";
            }
            else
            {
                // Create the Queue
                MessageQueue.Create(@".\Private$\AirportCompanySW");
                messageQueueSW = new MessageQueue(@".\Private$\AirportCompanySW");
                messageQueueSW.Label = "South West Queue";
            }

            string AirlineCompany = "SAS";

            ETAList etaList = null;

            etaList = new ETAList(AirlineCompany);

            AirlineReplyETA airlinereplyETA = null;

            airlinereplyETA = new AirlineReplyETA(requestETAQueue, etaList);

            while (true) { }

        }
    }
}

