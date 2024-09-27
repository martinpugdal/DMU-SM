using System;
using System.Collections.Generic;
using System.IO;
using System.Messaging;
using System.Xml;

namespace L11___Aggregator
{
    internal class Aggregator
    {
        private MessageQueue outputQueue;

        private readonly Dictionary<string, List<Message>> messageStore; // Store messages based on correlation ID
        private readonly Dictionary<string, int> expectedMessageCount; // Track expected number of messages for each correlation ID

        public Aggregator(MessageQueue messageQueue)
        {
            this.outputQueue = messageQueue;
            this.outputQueue.ReceiveCompleted += new ReceiveCompletedEventHandler(OnMessage);
            this.outputQueue.MessageReadPropertyFilter.CorrelationId = true;
            this.outputQueue.BeginReceive();
        }

        private void OnMessage(object source, ReceiveCompletedEventArgs asyncResult)
        {
            MessageQueue mq = (MessageQueue)source;
            Message message = mq.EndReceive(asyncResult.AsyncResult);

            Console.WriteLine(message.CorrelationId);
            Console.WriteLine(message.Id);

            // handle the messages now and add them to the dictionaries
            AddMessage(message);

            // Begin receiving messages
            mq.BeginReceive();
        }

        private void AddMessage(Message message)
        {
            // Extract correlation ID and total message count from the message
            string correlationId = GetCorrelationId(message);

            // not containing the correlation ID, then its a passenger message (always the first message)
            if (!messageStore.ContainsKey(correlationId))
            {
                messageStore[correlationId] = new List<Message>();
                int totalMessages = GetTotalMessageCountFromPassenger(message);
                expectedMessageCount[correlationId] = totalMessages; // Store the total number of expected messages
            }

            messageStore[correlationId].Add(message);

            // Check if the aggregation condition is met (i.e., all expected messages are received)
            if (CheckCompletness(correlationId))
            {
                AggregateAlgorithm(correlationId);
            }
        }

        private string GetCorrelationId(Message message)
        {
            return message.CorrelationId.Split('\\')[1];
        }

        private int GetTotalMessageCountFromPassenger(Message message)
        {
            // Set formatter to read the message body as a string
            message.Formatter = new XmlMessageFormatter(new String[] { "System.String,mscorlib" });

            // Convert message to xml document
            using (StreamReader reader = new StreamReader(message.BodyStream))
            {
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(reader.ReadToEnd().ToString());
                XmlNode node = xml.SelectSingleNode("/Passenger");

                // just in case the message is not a passenger message
                if (node == null)
                {
                    return -1; // error
                }

                string[] lugParts = node.SelectSingleNode("LuggageId").InnerText.Split('-');
                int luggageSize = int.Parse(lugParts[1]);
                return luggageSize + 1; // +1 for the passenger message
            }
        }

        /* Aggregation Completness, when are we ready to publish a result?
         * 
         * We use the mechanism "wait for all" to check if we have received all the messages.
         * Completeness is critical, its why "wait for all" is used.
        */
        private bool CheckCompletness(string correlationId)
        {
            return messageStore[correlationId].Count == expectedMessageCount[correlationId];
        }

        /* Aggregation Algorithm, How do we combine the messages?
         * 
         * We use the luggage size from passenger, so we know how many messages we should receive in total.
         * The order of the luggage is found by the messageId.
        */
        private void AggregateAlgorithm(string correlationId)
        {
            // Sort messages based on message ID
            messageStore[correlationId].Sort((m1, m2) => m1.Id.CompareTo(m2.Id));

            // Combine messages
            List<Message> messages = messageStore[correlationId];
            PublishResult(messages);

            // Clean up
            messageStore.Remove(correlationId);
            expectedMessageCount.Remove(correlationId);
        }

        private void PublishResult(List<Message> messages)
        {
            // what ever we want to do with the messages
            Console.WriteLine(messages.Count + " messages received and aggregated");
        }
    }
}
