using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Messaging;
using System.Xml;

// namespace BluffCityMultiCastPublisher
//{
    class AirlineCompanyPublisher
    {
        protected MessageQueue inQueue;
        protected MessageQueue outQueue;
        public AirlineCompanyPublisher(MessageQueue inQueue, MessageQueue multicastQueue)
        {
            this.inQueue = inQueue;
            this.outQueue = multicastQueue;
            inQueue.ReceiveCompleted += new ReceiveCompletedEventHandler(OnMessage);
            inQueue.BeginReceive();
            string label = inQueue.Label;

        }
        private void OnMessage(Object source, ReceiveCompletedEventArgs asyncResult)
        {
            MessageQueue mq = (MessageQueue)source;
            Message message = mq.EndReceive(asyncResult.AsyncResult);
            string label = message.Label;
            outQueue.Send(message);
            Console.WriteLine("Multicastbeskeder sendt");
            mq.BeginReceive();

        }
    }
//}
