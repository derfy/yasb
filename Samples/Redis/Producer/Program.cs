using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Autofac;
using Yasb.Common.Messaging;
using Yasb.Wireup;
using Yasb.Common.Messaging.EndPoints.Redis;
using Yasb.Common.Messaging.Configuration.Redis;
using Yasb.Common.Serialization;
using Yasb.Common.Serialization.Json;

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
            var configurator = new RedisConfigurator();
            var bus = configurator.Bus(sb => sb.EndPoints<RedisEndPointConfiguration>(lec => lec.ReceivesOn(c => c.WithHostName("192.168.227.128").WithQueueName("redis_producer")))
                                               .Serializer<JsonSerializationConfiguration<RedisEndPoint>>());
                                               //.ConfigureConnections<FluentIPEndPointConfigurer>(conn => conn.WithConnection("local", "192.168.227.128")
                                               //                                                              .WithConnection("ConsumerConnection", "192.168.227.128")));
           
            int i = 0;
            bus.Run();
            while (i < 5000)
            {
                Reset.WaitOne();
            //    i++;

                var message = new ExampleMessage(i, "I am ExampleMessage ");
                //bus.Send("consumer", message);
                bus.Publish(message);
                //i++;
               // bus.Send<ExampleMessage>("redis_consumer", message);
                var message2 = new ExampleMessage2(i, "I am ExampleMessage2");
                bus.Publish(message2);
               // bus.Send("consumer", message2);
                i++;
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
