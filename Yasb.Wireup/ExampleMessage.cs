using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging;
using System.Threading;

namespace Yasb.Wireup
{
    [Serializable]
    public class ExampleMessage2 : ExampleMessage
    {

        public ExampleMessage2(int number, string name)
            : base(number, name)
        {

        }

    }
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
            
           // Console.WriteLine(string.Format("Handler 1 : {0} {1}", message.Name, message.Number));
            Interlocked.Increment(ref _readCount);
        }

        public void Handle(ExampleMessage2 message)
        {
            //Console.WriteLine(string.Format("Handler 2 {0} {1}", message.Name, message.Number));
            //Thread.Sleep(2000);
           // throw new Exception("bu bu");
            
            Interlocked.Increment(ref _readCount);
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
