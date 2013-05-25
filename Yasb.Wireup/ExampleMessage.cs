using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging;
using System.Threading;

namespace Yasb.Wireup
{
    [Serializable]
    public class ExampleMessage : IMessage
    {

        public ExampleMessage(int number, string name)
        {
            Number = number;
            Name = name;
        }

        public int Number { get; set; }
        public string Name { get; set; }


    }

    public class ExampleMessageHandler : IHandleMessages<ExampleMessage>, IHandleMessages<ExampleMessage2>
    {
        private static int _readCount = 0;
        public static int ReadCount { get { return _readCount; } }
        public void Handle(ExampleMessage message)
        {
            Thread.Sleep(10);
            Console.WriteLine(string.Format("{0} {1}",message.Name,message.Number));

            Interlocked.Increment(ref _readCount);
        }

        public void Handle(ExampleMessage2 message)
        {
            Thread.Sleep(5);
           // throw new Exception("bu bu");
            Console.WriteLine(string.Format("{0} {1}", message.Name, message.Number));
            Interlocked.Increment(ref _readCount);
        }
    }

    [Serializable]
    public class ExampleMessage2 : ExampleMessage
    {

        public ExampleMessage2(int number, string name)
            : base(number, name)
        {

        }

    }
    public class ErrorMessage : IMessage
    {
        public int Number { get; private set; }
        public string Name { get; private set; }

        public ErrorMessage(int number, string name)
        {
            Number = number;
            Name = name;
        }

    }
}
