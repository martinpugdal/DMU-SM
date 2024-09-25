using System;
using System.Collections.Generic;

namespace L7___Messaging_Channels
{
    public class Publisher
    {
        private readonly List<Subscriber> subscribers = new List<Subscriber>();

        public void Publish(string message)
        {
            Console.WriteLine(message);
            NotifySubscribers(message);
        }

        public void NotifySubscribers(string message)
        {
            foreach (Subscriber subscriber in subscribers)
            {
                subscriber.Update(message);
            }
        }

        public void Subscribe(string airlinecompany)
        {
            Subscriber subscriber = new Subscriber(airlinecompany);
            subscribers.Add(subscriber);
        }

        public void Unsubscribe(string airlinecompany)
        {
            foreach (Subscriber subscriber in subscribers)
            {
                if (subscriber.Name.Equals(airlinecompany))
                {
                    subscribers.Remove(subscriber);
                    break;
                }
            }
        }
    }
}
