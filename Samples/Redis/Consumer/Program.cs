using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using System.Threading;
using Yasb.Common.Messaging;
using Yasb.Wireup;
using Yasb.Wireup.ConfiguratorExtensions.Redis;
using System.Net;
using Yasb.Common.Messaging.Configuration;
using Autofac.Builder;
using Yasb.Redis.Messaging.Configuration;
using Yasb.Common.Tests.Messages;
using Yasb.Redis.Messaging;

namespace Consumer
{
    internal class Program
    {
        private static readonly ManualResetEvent Reset = new ManualResetEvent(false);
        private static long _lastRead;
        private static long _readCount;
        private static Timer _timer;
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

            var ipAddress = "192.168.1.11";
            var bus = Configurator.Configure<RedisEndPoint>().ConfigureEndPoints(eb => eb.ReceivesOn(c => c.WithHostName(ipAddress).WithQueueName("redis_consumer"))
                                                   .SubscribesTo(string.Format("redis_producer@{0}",ipAddress), ec => ec.WithHostName(ipAddress).WithQueueName("redis_producer")))
                                                   .ConfigureSubscriptionService(cfg => cfg.WithHostName(ipAddress)).Bus();


            bus.Subscribe(string.Format("redis_producer@{0}", ipAddress));
            Console.WriteLine("subscription ExampleMessage sent");
            //bus.Subscribe("redis_producer@192.168.227.128");
            //Console.WriteLine("subscription ExampleMessage2 sent");
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
