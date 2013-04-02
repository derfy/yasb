using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasb.Wireup;
using Yasb.Common.Messaging;
using Yasb.Redis.Messaging;

namespace Yasb.Tests.Messaging.Redis
{
    /// <summary>
    /// Summary description for RedisQueueTest
    /// </summary>
    [TestClass]
    public class RedisQueueTest
    {
       

        [TestMethod]
        public void ShouldGetMessageOnlyOnce()
        {
            var configurator = new AutofacConfigurator();
            var queueFactory = configurator.Bus(c => c.WithLocalEndPoint("192.168.127.128:6379:queue_test")
                                             .WithMessageHandlersAssembly(typeof(TestMessage).Assembly))
                                   .Resolver().InstanceOf<Func<IEndPoint,IQueue>>();
            var endPoint = new RedisEndPoint("192.168.127.128:6379:queue_test");
            var queue = queueFactory(endPoint);
            var message=new TestMessage();
            var envelope = new MessageEnvelope(message, Guid.NewGuid().ToString(), queue.LocalEndPoint, queue.LocalEndPoint);
            //envelope.StartTime = DateTime.Now.Ticks - new TimeSpan(0, 1, 0).Ticks;
            queue.Push(envelope);

            MessageEnvelope newEnvelope = null;
            queue.Initialize();
            queue.TryGetEnvelope(TimeSpan.FromSeconds(5), out newEnvelope);
            Assert.IsNotNull(newEnvelope);
            queue.TryGetEnvelope(TimeSpan.FromSeconds(5), out newEnvelope);
            queue.TryGetEnvelope(TimeSpan.FromSeconds(5), out newEnvelope);
            queue.TryGetEnvelope(TimeSpan.FromSeconds(5), out newEnvelope);
            queue.TryGetEnvelope(TimeSpan.FromSeconds(5), out newEnvelope);
            queue.TryGetEnvelope(TimeSpan.FromSeconds(5), out newEnvelope);
            queue.TryGetEnvelope(TimeSpan.FromSeconds(5), out newEnvelope);
            queue.TryGetEnvelope(TimeSpan.FromSeconds(5), out newEnvelope);
            Assert.IsNull(newEnvelope);
        }
    }
}
