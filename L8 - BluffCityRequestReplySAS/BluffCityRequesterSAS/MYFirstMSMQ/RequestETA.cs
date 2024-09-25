using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Messaging;
using System.IO;
using System.Collections;
using System.Xml;
using System.Xml.Linq;

namespace BluffCityInformationCenterRouter
{
    class RequestETA
    {
        protected MessageQueue replyETC;
        protected MessageQueue requestAIC;
        protected ParserETA parseETA;

        public RequestETA(MessageQueue replyETC, MessageQueue requestAIC, ParserETA parseETA)
        {
            this.parseETA = parseETA;
            this.replyETC = replyETC;
            this.requestAIC = requestAIC;
            replyETC.ReceiveCompleted += new ReceiveCompletedEventHandler(OnMessage);
            replyETC.BeginReceive();
            string label = replyETC.Label;

        }
        private void OnMessage(Object source, ReceiveCompletedEventArgs asyncResult)
        {
            MessageQueue mq = (MessageQueue)source;
            Message message = mq.EndReceive(asyncResult.AsyncResult);
            string label = message.Label;

            string text = parseETA.formaterETA(message);

            mq.BeginReceive();

        }
    }
}
