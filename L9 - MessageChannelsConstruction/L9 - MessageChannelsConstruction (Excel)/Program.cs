using System;
using System.Messaging;
using System.Text.Json;

namespace L9___MessageChannelsConstruction__Excel_
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // setup the message queue
            MessageQueue messageQueue = new MessageQueue()
            {
                Formatter = new XmlMessageFormatter(new String[] { "System.String,mscorlib" }),
                Path = @".\private$\L9Excel"
            };
            // create the message queue if it does not exist
            if (!MessageQueue.Exists(messageQueue.Path))
            {
                MessageQueue.Create(messageQueue.Path);
            }

            // create the router to receive messages
            new Router(messageQueue);

            // send a message
            AirlineCompany airlineCompany1 = new AirlineCompany(
                "SAS", 
                "CPH", 
                "SK123", 
                "JFK", 
                DateTime.Now
            );

            string json = JsonSerializer.Serialize(airlineCompany1);
            //for (var i = 0; i < 10; i++)
            //{
            //    messageQueue.Send(json);
            //}
            messageQueue.Send(json);

            while (true) { } // keep the program running (to receive messages   
        }
    }
}
