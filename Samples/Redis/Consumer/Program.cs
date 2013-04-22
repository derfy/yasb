﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using System.Threading;
using Yasb.Common.Messaging;
using Yasb.Wireup;
using Yasb.Common.Messaging.Configuration.CommonConnectionConfigurers;

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
            var bus = configurator.Bus(sb => sb.WithEndPointConfiguration(ec => ec.WithLocalEndPoint("ConsumerConnection", "redis_consumer")
                .WithEndPoint("ConsumerConnection", "redis_producer", "producer")).WithMessageHandlersAssembly(typeof(ExampleMessage).Assembly)
                .ConfigureConnections<FluentIPEndPointConfigurer>(c => c.WithConnection("ConsumerConnection", "192.168.127.128")));
            
            


            bus.Subscribe<ExampleMessage>("producer");
            bus.Subscribe<ExampleMessage2>("producer");
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