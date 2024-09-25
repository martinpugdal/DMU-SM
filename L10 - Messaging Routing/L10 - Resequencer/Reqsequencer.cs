using System;
using System.Collections.Generic;
using System.IO;
using System.Messaging;
using System.Xml;

namespace L10___Resequencer
{
    internal class Reqsequencer
    {
        private MessageQueue passengerQueue;
        private MessageQueue luggageQueue;
        private MessageQueue outputQueue;

        private Dictionary<int, XmlDocument> passengers = new Dictionary<int, XmlDocument>();
        private Dictionary<int, List<XmlDocument>> luggages = new Dictionary<int, List<XmlDocument>>();

        public Reqsequencer(MessageQueue outputQueue, MessageQueue passengerQueue, MessageQueue luggageQueue)
        {
            this.outputQueue = outputQueue;
            this.passengerQueue = passengerQueue;
            this.luggageQueue = luggageQueue;

            this.passengerQueue.ReceiveCompleted += new ReceiveCompletedEventHandler(OnMessage);
            this.luggageQueue.ReceiveCompleted += new ReceiveCompletedEventHandler(OnMessage);

            this.passengerQueue.BeginReceive();
            this.luggageQueue.BeginReceive();
        }

        private void OnMessage(Object source, ReceiveCompletedEventArgs asyncResult)
        {
            MessageQueue mq = (MessageQueue)source;
            Message message = mq.EndReceive(asyncResult.AsyncResult);

            XmlDocument xml = new XmlDocument();

            // Convert message to xml document
            using (StreamReader reader = new StreamReader(message.BodyStream))
            {
                xml.LoadXml(reader.ReadToEnd().ToString());

                XmlNode node = xml.SelectSingleNode("/Passenger");
                // check if the message is a passenger
                if (node != null)
                {
                    Console.WriteLine("Passenger: 1");
                    RunPassenger(xml);
                }
                else
                {
                    Console.WriteLine("Luggage: 1");
                    node = xml.SelectSingleNode("/Luggage");
                    // check if the message is a luggage
                    if (node != null)
                    {
                        Console.WriteLine("Luggage: 2");
                        RunLuggage(xml);
                    }
                }

                mq.BeginReceive();
            }
        }


        private void RunPassenger(XmlDocument passengerXml)
        {

            XmlNode passengerNode = passengerXml.SelectSingleNode("/Passenger");
            if (passengerNode != null)
            {
                Console.WriteLine(passengerNode.OuterXml);
                string[] lugParts = passengerNode.SelectSingleNode("LuggageId").InnerText.Split('-');
                int seqNo = int.Parse(lugParts[0]);
                int luggageSize = int.Parse(lugParts[1]);

                Console.WriteLine("SeqNo: " + seqNo);
                Console.WriteLine("LuggageSize: " + luggageSize);

                passengers.Add(seqNo, passengerXml);

                CheckRequestFilled(seqNo);

                passengerQueue.BeginReceive();
                Console.WriteLine(passengerXml.OuterXml);
            }
        }

        private void RunLuggage(XmlDocument luggageXml)
        {

            XmlNode luggageNode = luggageXml.SelectSingleNode("/Luggage");
            if (luggageNode != null)
            {
                int PassengerId = int.Parse(luggageNode.SelectSingleNode("PassengerId").InnerText);
                int luggageId = int.Parse(luggageNode.SelectSingleNode("Identification").InnerText);

                Console.WriteLine("PassengerId: " + PassengerId);
                Console.WriteLine("LuggageId: " + luggageId);

                if (luggages.ContainsKey(PassengerId))
                {
                    luggages[PassengerId].Add(luggageXml);
                }
                else
                {
                    luggages.Add(PassengerId, new List<XmlDocument> { luggageXml });
                }

                CheckRequestFilled(PassengerId);

                luggageQueue.BeginReceive();
                Console.WriteLine(luggageXml.OuterXml);
            }
        }

        private void CheckRequestFilled(int seqNo)
        {
            passengers.TryGetValue(seqNo, out XmlDocument passengerXml);
            if (passengerXml == null) return;

            XmlNode passengerNode = passengerXml.SelectSingleNode("/Passenger");
            int luggageSize = int.Parse(passengerNode.SelectSingleNode("LuggageId").InnerText.Split('-')[1]);
            if (!luggages.ContainsKey(seqNo) || luggages[seqNo].Count < luggageSize) return;

            // Send the passenger and luggage details to the output queue in the correct order
            Message passengerMessage = new Message(passengerXml.OuterXml)
            {
                Formatter = new XmlMessageFormatter(new String[] { "System.String,mscorlib" })
            };

            string _correlationId = passengerMessage.CorrelationId;
            outputQueue.Send(passengerMessage);

            foreach (var luggageXml in luggages[seqNo])
            {
                Message luggageMessage = new Message(luggageXml.OuterXml)
                {
                    Formatter = new XmlMessageFormatter(new String[] { "System.String,mscorlib" }),
                    CorrelationId = _correlationId
                };
                outputQueue.Send(luggageMessage);
            }

            passengers.Remove(seqNo);
            luggages.Remove(seqNo);
        }
    }
}