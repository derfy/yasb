﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Yasb.Common.Tests.Messages;
using Yasb.MongoDb.Messaging;
using Yasb.MongoDb.Messaging.Configuration;
using Yasb.Wireup;
using Yasb.Wireup.ConfiguratorExtensions.MongoDb;
namespace Producer
{
    class Program
    {
        private static readonly ManualResetEvent Reset = new ManualResetEvent(false);
        private static long _lastWrite;
        private static long _writeCount;
        private static Timer _timer;
        private static readonly object Sync = new object();

        private static void Main(string[] args)
        {
            Console.WriteLine("Publisher");
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
             var bus = Configurator.Configure<MongoDbEndPoint>().ConfigureEndPoints(c => c.ReceivesOn(cfg => cfg.WithHostName("vmEndPoint").WithQueueName("msmq_producer"))).Bus();
                                               //.ConfigureConnections<MongoDbFluentConnectionConfigurer>(c => c.WithConnection("vmEndPoint", "192.168.127.128", "test")));

            int i = 0;
            bus.Run();



            while (i < 5000)
            {
                Reset.WaitOne();
                i++;
                var message = new ExampleMessage(i, "I am Handler 1 ");
                //bus.Send("consumer", message);
                bus.Publish(message);
                i++;
                // bus.Send<ExampleMessage>("redis_consumer", message);
                var message2 = new ExampleMessage2(i, "I am Handler 2");
                bus.Publish(message2);
                // bus.Send("consumer", message2);
                Interlocked.Increment(ref _writeCount);
            }

        }

        public static void TickTock(object state)
        {
            lock (Sync)
            {
                Console.WriteLine("Sent {0} (total {1})", _writeCount - _lastWrite, _writeCount);
                _lastWrite = _writeCount;
            }
        }
    }
}
