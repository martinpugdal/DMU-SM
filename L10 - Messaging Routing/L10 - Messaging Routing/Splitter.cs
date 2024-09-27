using System;
using System.Collections.Generic;
using System.IO;
using System.Messaging;
using System.Xml;

namespace L10___Messaging_Routing
{
    class Splitter
    {
        private MessageQueue messageQueue;
        private MessageQueue luggageQueue = new MessageQueue(@".\Private$\AirportLuggageInput");
        private MessageQueue checkInQueue = new MessageQueue(@".\Private$\AirportCheckInInput");
        private int lastSeqNo = 0;
        public Splitter(MessageQueue messageQueue)
        {
            if (!MessageQueue.Exists(@".\Private$\AirportLuggageInput"))
            {
                MessageQueue.Create(@".\Private$\AirportLuggageInput");
            }
            if (!MessageQueue.Exists(@".\Private$\AirportCheckInInput"))
            {
                MessageQueue.Create(@".\Private$\AirportCheckInInput");
            }

            this.messageQueue = messageQueue;
            this.messageQueue.ReceiveCompleted += new ReceiveCompletedEventHandler(OnMessage);
            this.messageQueue.BeginReceive();
        }

        private void OnMessage(object source, ReceiveCompletedEventArgs asyncResult)
        {
            MessageQueue mq = (MessageQueue)source;
            Message message = mq.EndReceive(asyncResult.AsyncResult);

            // convert the message to xml document
            XmlDocument xml = new XmlDocument();
            StreamReader reader = new StreamReader(message.BodyStream);
            xml.LoadXml(reader.ReadToEnd().ToString());

            XmlNode itemNode = xml.SelectSingleNode("/FlightDetailsInfoResponse");
            if (itemNode != null)
            {
                XmlDocument passengerXml = new XmlDocument();
                passengerXml.LoadXml(itemNode.SelectSingleNode("Passenger").OuterXml);

                // get the passenger details
                XmlNode passengerNode = itemNode.SelectSingleNode("Passenger");
                if (passengerNode != null)
                {
                    string reservationNumber = passengerNode.SelectSingleNode("ReservationNumber").InnerText;
                    if (reservationNumber != null)
                        Console.WriteLine("ReservationNumber: " + reservationNumber);
                    string passengerFirstName = passengerNode.SelectSingleNode("FirstName").InnerText;
                    if (passengerFirstName != null)
                        Console.WriteLine("FirstName: " + passengerFirstName);
                    string passengerLastName = passengerNode.SelectSingleNode("LastName").InnerText;
                    if (passengerLastName != null)
                        Console.WriteLine("LastName: " + passengerLastName);
                }

                // update the sequence number
                int thisSeqNo = lastSeqNo++;

                // get the luggages details
                List<XmlDocument> luggagesXml = new List<XmlDocument>();
                XmlNodeList luggages = itemNode.SelectNodes("Luggage");
                if (luggages != null)
                {
                    foreach (XmlNode luggage in luggages)
                    {
                        // create a new xml document for each luggage
                        XmlDocument luggageXml = new XmlDocument();
                        luggageXml.LoadXml(luggage.OuterXml);

                        // add passenger ID to the luggage
                        XmlNode luggagePassengerId = luggageXml.CreateNode(XmlNodeType.Element, "PassengerId", "");
                        luggagePassengerId.InnerText = thisSeqNo.ToString();
                        luggageXml.DocumentElement.AppendChild(luggagePassengerId);
                        luggagesXml.Add(luggageXml);

                        string luggageId = luggage.SelectSingleNode("Id").InnerText;
                        if (luggageId != null)
                            Console.WriteLine("Id: " + luggageId);
                        string identification = luggage.SelectSingleNode("Identification").InnerText;
                        if (identification != null)
                            Console.WriteLine("Identification: " + identification);
                        string luggageWeight = luggage.SelectSingleNode("Weight").InnerText;
                        if (luggageWeight != null)
                            Console.WriteLine("Weight: " + luggageWeight);
                    }
                }

                // add luggages size to the passenger
                XmlNode xmlNode = passengerXml.CreateNode(XmlNodeType.Element, "LuggageId", "");
                xmlNode.InnerText = thisSeqNo + "-" + luggagesXml.Count;
                passengerXml.DocumentElement.AppendChild(xmlNode);

                // send the passenger details to the check-in service
                checkInQueue.Send(passengerXml);

                // send the luggage details to the luggage service
                foreach (XmlDocument luggageXml in luggagesXml)
                {
                    luggageQueue.Send(luggageXml);
                }
            }

            mq.BeginReceive();
        }
    }
}