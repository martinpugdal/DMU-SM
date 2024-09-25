using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Messaging;

namespace BluffCityInformationCenterETA
{
    class AirlineReplyETA
    {
        protected MessageQueue inQueue;
        protected MessageQueue replyETA = null;
        protected ETAList etaList = null;

        //        public AirlineReplyETA(MessageQueue inQueue, MessageQueue outQueueSAS, MessageQueue outQueueKLM, MessageQueue outQueueSW)
        public AirlineReplyETA(MessageQueue inQueue, ETAList etaList)
        {
            this.inQueue = inQueue;
            this.etaList = etaList;

            inQueue.ReceiveCompleted += new ReceiveCompletedEventHandler(OnMessage);
            inQueue.BeginReceive();
            string label = inQueue.Label;

        }
        private void OnMessage(Object source, ReceiveCompletedEventArgs asyncResult)
        {

            MessageQueue mq = (MessageQueue)source;
            Message message = mq.EndReceive(asyncResult.AsyncResult);
            string label = message.Label;
            string eta;

            eta = etaList.ReturnETA(label);

            Console.WriteLine(label);
            Console.WriteLine(eta);

            replyETA = new MessageQueue(@".\Private$\ReplySASETA");
            replyETA.Label = "ReplyAICETA Queue";

            Message replyMessage = new Message();
            replyMessage.Body = eta;

            replyMessage.Label = label;
            replyETA.Send(replyMessage);

            mq.BeginReceive();

        }
    }
}
