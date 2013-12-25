using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using System.Threading;
using Yasb.Common.Messaging;
using Yasb.Wireup;
using System.Net;
using Yasb.Common.Messaging.Configuration;
using Autofac.Builder;
using Yasb.Common.Messaging.EndPoints.Redis;
using Yasb.Common.Messaging.Configuration.Redis;
using Yasb.Common.Serialization;
using Yasb.Common.Serialization.Json;
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
            

            var configurator = new RedisConfigurator();
            var bus = configurator.Bus(sb => sb.EndPoints<RedisEndPointConfiguration>(eb => eb.ReceivesOn(c => c.WithHostName("192.168.227.128").WithQueueName("redis_consumer"))
                                                   .SubscribesTo("redis_producer@192.168.227.128", ec => ec.WithHostName("192.168.227.128").WithQueueName("redis_producer")))
                                                .Serializer<JsonSerializationConfiguration<RedisEndPoint>>());


            bus.Subscribe<ExampleMessage>("redis_producer@192.168.227.128");
            Console.WriteLine("subscription ExampleMessage sent");
            bus.Subscribe<ExampleMessage2>("redis_producer@192.168.227.128");
            Console.WriteLine("subscription ExampleMessage2 sent");
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
