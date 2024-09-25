using System;

namespace L7___Messaging_Channels
{
    internal class Subscriber : ISubscriber
    {

        public string Name { get; }


        public Subscriber(string name)
        {
            this.Name = name;
        }

        public void Update(string data)
        {
            Console.WriteLine(Name + ": " + data);
        }
    }
}
