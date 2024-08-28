using System;
using System.Collections.Generic;
using System.Messaging;
using Newtonsoft.Json;


namespace L2___Messaging_System_og_Channels
{
    class AirlineCenterRouter
    {
        protected MessageQueue airlineInfoCenterMsgQueue;
        protected Dictionary<string, MessageQueue> airlinesCompanyMsgQueue;
        public AirlineCenterRouter(MessageQueue airlineInfoCenterMsgQueue, Dictionary<string, MessageQueue> airlinesCompanyMsgQueue)
        {
            this.airlineInfoCenterMsgQueue = airlineInfoCenterMsgQueue;
            this.airlinesCompanyMsgQueue = airlinesCompanyMsgQueue;
            airlineInfoCenterMsgQueue.ReceiveCompleted += new ReceiveCompletedEventHandler(OnMessage);
            airlineInfoCenterMsgQueue.BeginReceive();
        }
        private void OnMessage(Object source, ReceiveCompletedEventArgs asyncResult)
        {
            // cast source to msgQueue and get the message
            MessageQueue mq = (MessageQueue)source;
            Message message = mq.EndReceive(asyncResult.AsyncResult);
            Console.WriteLine(message.Body);

            // get the airline company message queue from the map
            string airlineCompany = message.Label;
            if (!airlinesCompanyMsgQueue.ContainsKey(airlineCompany))
            {
                // airline company not found
                mq.BeginReceive();
                return;
            }

            // redirect the message to the airline's msg queue
            MessageQueue airlineCompanyMsgQueue = airlinesCompanyMsgQueue[airlineCompany];
            airlineCompanyMsgQueue.Send(message);


            mq.BeginReceive();
        }
    }
}
