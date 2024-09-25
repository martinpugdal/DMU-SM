using System;
using System.Messaging;

namespace L9___MessageChannelsConstruction
{
    internal class Program
    {
        static void Main(string[] args)
        {
            MessageQueue messageQueue = new MessageQueue()
            {
                Formatter = new XmlMessageFormatter(new String[] { "System.String,mscorlib" }),
                Path = @".\private$\L9MessageQueue"
            };

            // Create the message queue if it does not exist
            if (!MessageQueue.Exists(messageQueue.Path))
            {
                MessageQueue.Create(messageQueue.Path);
            }

            Message message = new Message("Hello, World!")
            {
                TimeToBeReceived = new TimeSpan(0, 0, 5)
            };
            messageQueue.Send(message);


            Console.WriteLine("Before 5 seconds");
            Message[] messages = messageQueue.GetAllMessages();
            foreach (Message msg in messages)
            {
                Console.WriteLine(msg.Body.ToString());
            }

            System.Threading.Thread.Sleep(5000);

            Console.WriteLine("After 5 seconds");
            messages = messageQueue.GetAllMessages();
            foreach (Message msg in messages)
            {
                Console.WriteLine(msg.Body.ToString());
            }
            if (messages.Length == 0)
            {
                Console.WriteLine("Empty queue");
            }


            Console.ReadKey();
        }
    }
}
