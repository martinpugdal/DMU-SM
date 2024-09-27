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
        private Dictionary<int, Dictionary<int, XmlDocument>> luggages = new Dictionary<int, Dictionary<int, XmlDocument>>();
        
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
            message.Formatter = new XmlMessageFormatter(new String[] { "System.String,mscorlib" });

            XmlDocument xml = new XmlDocument();

            // Convert message to xml document
            using (StreamReader reader = new StreamReader(message.BodyStream))
            {
                xml.LoadXml(reader.ReadToEnd().ToString());

                XmlNode node = xml.SelectSingleNode("/Passenger");
                // check if the message is a passenger
                if (node != null)
                {
                    RunPassenger(xml);
                }
                else
                {
                    node = xml.SelectSingleNode("/Luggage");
                    // check if the message is a luggage
                    if (node != null)
                    {
                        RunLuggage(xml);
                    }
                }

                // Continue receiving messages
                mq.BeginReceive();
            }
        }


        private void RunPassenger(XmlDocument passengerXml)
        {

            XmlNode passengerNode = passengerXml.SelectSingleNode("/Passenger");
            if (passengerNode != null)
            {
                string[] lugParts = passengerNode.SelectSingleNode("LuggageId").InnerText.Split('-');
                int seqNo = int.Parse(lugParts[0]);
                int luggageSize = int.Parse(lugParts[1]);

                Console.WriteLine("Passenger/SeqNo: " + seqNo);
                Console.WriteLine("Passenger/LuggageSize: " + luggageSize);

                passengers.Add(seqNo, passengerXml);

                CheckRequestFilledAndSend(seqNo);
            }
        }

        private void RunLuggage(XmlDocument luggageXml)
        {

            XmlNode luggageNode = luggageXml.SelectSingleNode("/Luggage");
            if (luggageNode != null)
            {
                int PassengerId = int.Parse(luggageNode.SelectSingleNode("PassengerId").InnerText);
                int luggageId = int.Parse(luggageNode.SelectSingleNode("Identification").InnerText);

                Console.WriteLine("Luggage/PassengerId: " + PassengerId);
                Console.WriteLine("Luggage/Id: " + luggageId);

                if (luggages.ContainsKey(PassengerId))
                {
                    // Insert luggage in correct order based on luggageId
                    luggages[PassengerId].Add(luggageId, luggageXml);
                }
                else
                {
                    luggages.Add(PassengerId, new Dictionary<int, XmlDocument> { { luggageId, luggageXml } });
                }

                CheckRequestFilledAndSend(PassengerId);
            }
        }

        private void CheckRequestFilledAndSend(int seqNo)
        {
            passengers.TryGetValue(seqNo, out XmlDocument passengerXml);
            if (passengerXml == null) return;

            XmlNode passengerNode = passengerXml.SelectSingleNode("/Passenger");
            int luggageSize = int.Parse(passengerNode.SelectSingleNode("LuggageId").InnerText.Split('-')[1]);
            if (!luggages.ContainsKey(seqNo) || luggages[seqNo].Count < luggageSize) return;

            Console.WriteLine("Passenger and luggage details filled for seqNo: " + seqNo);

            // Create correlationId
            //string uuid = Guid.NewGuid().ToString();
            string uuid = "00000000-0000-0000-0000-000000000000";
            string correlationId = uuid + @"\" + seqNo;

            // Send the passenger and luggage details to the output queue in the correct order
            Message passengerMessage = new Message(passengerXml.OuterXml)
            {
                Formatter = new XmlMessageFormatter(new String[] { "System.String,mscorlib" }),
                CorrelationId = correlationId
            };

            outputQueue.Send(passengerMessage);


            foreach (var luggageXml in luggages[seqNo].Values)
            {
                Message luggageMessage = new Message(luggageXml.OuterXml)
                {
                    Formatter = new XmlMessageFormatter(new String[] { "System.String,mscorlib" }),
                    CorrelationId = correlationId
                };
                outputQueue.Send(luggageMessage);
            }

            Console.WriteLine("Passenger and luggage details sent to the output queue in the correct order with seqNo: " + seqNo);
            Console.WriteLine("Passenger: " + passengerXml.OuterXml);
            Console.WriteLine("Luggages: ");
            foreach (var luggageXml in luggages[seqNo].Values)
            {
                Console.WriteLine(luggageXml.OuterXml);
            }
            Console.WriteLine();

            passengers.Remove(seqNo);
            luggages.Remove(seqNo);
        }
    }
}