using System;
using System.Text;
using System.Linq;
using System.Xml.Linq;
using System.Messaging;

namespace BluffCityInformationCenterRouter
{
    class Program
    {         
        static void Main(string[] args)
        {

            MessageQueue replyETC = null;
            if (MessageQueue.Exists(@".\Private$\ReplySASETA"))
            {
                replyETC = new MessageQueue(@".\Private$\ReplySASETA");
                replyETC.Label = "ReplyAICETA Queue";
            }
            else
            {
                // Create the Queue
                MessageQueue.Create(@".\Private$\ReplySASETA");
                replyETC = new MessageQueue(@".\Private$\ReplySASETA");
                replyETC.Label = "ReplyAICETA Queue";
            }

            MessageQueue requestAIC = null;
            if (MessageQueue.Exists(@".\Private$\RequestETAQueue"))
            {
                requestAIC = new MessageQueue(@".\Private$\RequestETAQueue");
                requestAIC.Label = "SAS Queue";
            }
            else
            {
                // Create the Queue
                MessageQueue.Create(@".\Private$\RequestETAQueue");
                requestAIC = new MessageQueue(@".\Private$\RequestETAQueue");
                requestAIC.Label = "SAS Queue";
            }

            Message requestMessage = new Message();

            //string Airline = "SK942";
            string Airline = "KL105";
            
            requestMessage.Body = Airline;
            requestMessage.Label = Airline.Substring(0, 5);

            requestMessage.ResponseQueue = replyETC;
            requestAIC.Send(requestMessage);

            RequestETA requestETA = null;

            ParserETA parseETA = null;
            parseETA = new ParserETA();

            requestETA = new RequestETA(replyETC, requestAIC, parseETA);
            while (true) { } 
        }
    }
}

