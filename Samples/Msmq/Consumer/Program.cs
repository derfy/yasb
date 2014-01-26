using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Wireup;
using System.Threading;
using Yasb.Msmq.Messaging.Configuration;
using Yasb.Common.Tests.Messages;
namespace Consumer
{
    internal class Program
    {
        private static readonly ManualResetEvent Reset = new ManualResetEvent(false);
        private static long _lastRead;
        private static long _readCount;
        private static System.Threading.Timer _timer;
        private static readonly object Sync = new object();

        private static void Main(string[] args)
        {
            Console.WriteLine("Subscriber");
            Console.WriteLine("Press 'R' to Run, 'P' to Pause, 'X' to Exit ...");

            _timer = new Timer(TickTock, null, 1000, 1000);

            var t = new Thread(Run);
            t.Start();

            bool running = true;
            while (running)
            {
                if (!Console.KeyAvailable) continue;

                ConsoleKeyInfo keypress = Console.ReadKey(true);
                switch (keypress.Key)
                {
                    case ConsoleKey.X:
                        Reset.Reset();
                        running = false;
                        break;
                    case ConsoleKey.P:
                        Reset.Reset();
                        Console.WriteLine("Paused ...");
                        break;
                    case ConsoleKey.R:
                        Reset.Set();
                        Console.WriteLine("Running ...");
                        break;
                }
            }

            t.Abort();
        }

        public static void Run()
        {
            var configurator = new MsmqConfigurator();
            var bus = configurator.Bus(sb => sb.EndPoints(cfg => cfg.ReceivesOn(c => c.WithQueueName("msmq_consumer"))
                                                                                               .SubscribesTo("msmq_producer", ec => ec.WithQueueName("msmq_producer")))
                                               .ConfigureSubscriptionService(c => c.WithHostName("192.168.227.128").WithDatabase("Subscriptions")));

            bus.Subscribe("msmq_producer");
            bus.Subscribe("msmq_producer");
            bus.Run();
        }

        public static void TickTock(object state)
        {
            lock (Sync)
            {
                Console.WriteLine("Received {0} (total {1})", ExampleMessageHandler.ReadCount - _lastRead, ExampleMessageHandler.ReadCount);
                _lastRead = ExampleMessageHandler.ReadCount;
            }
        }
    }
}
