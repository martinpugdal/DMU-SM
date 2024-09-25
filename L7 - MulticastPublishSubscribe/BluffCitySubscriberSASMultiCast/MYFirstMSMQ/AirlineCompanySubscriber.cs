using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Messaging;

namespace MYFirstMSMQ
{
    class AirlineCompanySubscriber
    {
        protected MessageQueue inQueue;

        public AirlineCompanySubscriber(MessageQueue inQueue)
        {

            inQueue.ReceiveCompleted += new ReceiveCompletedEventHandler(OnMessage);
            inQueue.BeginReceive();         

        }
        private void OnMessage(Object source, ReceiveCompletedEventArgs asyncResult)
        {
            MessageQueue mq = (MessageQueue)source;
            Message message = mq.EndReceive(asyncResult.AsyncResult);
            Console.WriteLine("Her kan vi indsætte et filter");
            string label = message.Label;
            Console.WriteLine(label);
            mq.BeginReceive();

        }
    }
}
