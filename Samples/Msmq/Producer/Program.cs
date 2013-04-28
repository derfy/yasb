using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Wireup;
using Yasb.Msmq.Messaging.Configuration;
using Yasb.Common.Messaging;
using System.Threading.Tasks;
using Yasb.Common.Tests;
using System.Threading;

namespace Producer
{
    class Program
    {
        static void Main(string[] args)
        {
            var sut = new MsmqConfigurator().ConfigureQueue(e=>e.WithEndPoint("localConnection","test_msmq_local","queue1")
                .ConfigureConnections<MsmqFluentConnectionConfigurer>(c => c.WithConnection("localConnection", "localhost"))).CreateFromEndPointName("queue1");
            //var localEndPoint = new BusEndPoint("localConnection:test_msmq_local");
            //var remoteEndPoint = new BusEndPoint("localConnection:test_msmq_remote");

            //for (int i = 0; i < 10000; i++)
            //{
            //    var message = new TestMessage(i);
            //    var messageEnvelope = new MessageEnvelope(message, localEndPoint, remoteEndPoint);
            //    sut.Push(messageEnvelope);
            //}
           
           //var converters = new List<JsonConverter>() {  new MessageEnvelopeConverter<MsmqEndPoint>() }.ToArray();
           // var serializer = new Serializer(converters);
           // var formatter = new JsonMessageFormatter<MessageEnvelope>(serializer);
           // var sut = new MsmqQueue(localEndPoint, formatter);
            // sut.Initialize();
             

           // sut.TryGetEnvelope(DateTime.Now, new TimeSpan(0, 0, 50), out messageEnvelope);
           // Assert.IsNotNull(messageEnvelope);
           // sut.SetMessageCompleted(messageEnvelope.Id);
           // sut.TryGetEnvelope(DateTime.Now, new TimeSpan(0, 0, 50), out messageEnvelope);
           // Assert.IsNull(messageEnvelope);
            var tasks = new Task[10000];
            for (int i = 0; i < 10000; i++)
            {
                tasks[i] = Task.Factory.StartNew((current) =>
                {
                    Thread.CurrentThread.Name = current.ToString();
                    Dequeue(sut);
                },i);
            }
            Task.WaitAll(tasks);
            //Dequeue(sut);
            //Task.Factory.StartNew(() =>
            //{
            //    Dequeue(sut);
            //});
            //Task.Factory.StartNew(() =>
            //{
            //    Dequeue(sut);
            //});
        }

        private static void Dequeue(IQueue sut)
        {
            MessageEnvelope env = null;
            if (sut.TryDequeue(DateTime.Now, new TimeSpan(0, 0, 5), out env)) {
                var message = env.Message as TestMessage;
                Console.WriteLine(env.StartTimestamp);
                sut.SetMessageCompleted(env.Id);
            }
           
        }
        
    }
}
