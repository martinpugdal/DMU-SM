using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Messaging;

namespace L7___Messaging_Channels
{
    class AirlineCenterRouter
    {
        protected MessageQueue airlineInfoCenterMsgQueue;
        protected Dictionary<string, MessageQueue> airlinesCompanyMsgQueue;
        protected Publisher publisher;

        public AirlineCenterRouter(MessageQueue airlineInfoCenterMsgQueue, Dictionary<string, MessageQueue> airlinesCompanyMsgQueue, Publisher publisher)
        {
            this.airlineInfoCenterMsgQueue = airlineInfoCenterMsgQueue;
            this.airlinesCompanyMsgQueue = airlinesCompanyMsgQueue;
            this.publisher = publisher;
            airlineInfoCenterMsgQueue.ReceiveCompleted += new ReceiveCompletedEventHandler(OnMessage);
            airlineInfoCenterMsgQueue.BeginReceive();
        }
        private void OnMessage(Object source, ReceiveCompletedEventArgs asyncResult)
        {
            // cast source to msgQueue and get the message
            MessageQueue mq = (MessageQueue)source;
            Message message = mq.EndReceive(asyncResult.AsyncResult);

            // get the airline company message queue from the map
            string airlineCompany = message.Label;
            if (!airlinesCompanyMsgQueue.ContainsKey(airlineCompany))
            {
                // airline company not found
                mq.BeginReceive();
                return;
            }

            // sending the message to the airlinescompany
            foreach (MessageQueue queue in airlinesCompanyMsgQueue.Values)
            {
                queue.Send(message, message.Label);
                Console.WriteLine("Sent message to " + queue.Label);
            }
            // publish, so subscribers will be updated
            publisher.Publish(message.Body.ToString());

            mq.BeginReceive();
        }
    }
}
