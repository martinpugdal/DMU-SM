using System;
using System.IO;
using System.Messaging;
using System.Xml;

//namespace MYFirstMSMQ
//{
class AirlineCompanyXPath
{
    protected MessageQueue inQueue;


    public AirlineCompanyXPath(MessageQueue inQueue)
    {
        this.inQueue = inQueue;

        inQueue.ReceiveCompleted += new ReceiveCompletedEventHandler(OnMessage);
        inQueue.BeginReceive();
        string label = inQueue.Label;

    }
    private void OnMessage(Object source, ReceiveCompletedEventArgs asyncResult)
    {
        MessageQueue mq = (MessageQueue)source;
        Message message = mq.EndReceive(asyncResult.AsyncResult);
        string label = message.Label;
        XmlDocument xml = new XmlDocument();
        string XMLDocument = null;
        Console.WriteLine(label);
        Stream body = message.BodyStream;
        StreamReader reader = new StreamReader(body);
        XMLDocument = reader.ReadToEnd().ToString();
        xml.LoadXml(XMLDocument);
        XmlNode itemNode = xml.SelectSingleNode("/AirportInfoGate/airline/Flight");
        if (itemNode != null)
        {
            XmlNode value = itemNode.SelectSingleNode("Gate");
            if (value != null)
            {
                String valueString = value.Attributes["No"].Value;
                Console.WriteLine("Længde : " + valueString.Length);
                if (valueString != null)
                {
                    Console.WriteLine("GateNo : " + valueString);
                }
            }
        }
        Console.WriteLine("Besked sendt");
        mq.BeginReceive();

    }
}
//}
