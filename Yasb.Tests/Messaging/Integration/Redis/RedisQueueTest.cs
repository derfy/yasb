using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasb.Wireup;
using Yasb.Common.Messaging;
using Yasb.Redis.Messaging;
using Yasb.Redis.Messaging.Client;
using Yasb.Common.Tests;
using Yasb.Common.Messaging.Configuration.CommonConnectionConfigurers;
using Autofac;
using System.Net;
using Yasb.Redis.Messaging.Client.Interfaces;
using System.Threading.Tasks;
using Yasb.Tests.Messaging.Integration.Redis;
using System.Threading.Tasks;

namespace Yasb.Tests.Messaging.Redis
{
    
    /// <summary>
    /// Summary description for RedisQueueTest
    /// </summary>
    [TestClass]
    public class RedisQueueTest
    {
        private IQueue _queue;
        private IRedisClient _redisClient;
        public RedisQueueTest()
        {
            var configurator = new RedisConfigurator();
            var redisQueueFactory = configurator.ConfigureQueue(q => q.WithEndPoint("vmEndPoint", "queue_test", "consumer")
                                             .ConfigureConnections<FluentIPEndPointConfigurer>(c => c.WithConnection("vmEndPoint", "192.168.127.128")));
            _queue = redisQueueFactory.CreateFromEndPointName("consumer") as RedisQueue;
            _redisClient=configurator.ConfigureRedisClient("192.168.127.128");
        }
        [TestInitialize()]
        public void BeforeTest()
        {
            _redisClient.Del("queue_test");
            
        }
      //  [Ignore]
        [TestMethod]
        public void ShouldSetTimeoutError()
        {
            var message = new TestMessage("This is a test");
            _queue.Push(message, _queue.LocalEndPoint);
            MessageEnvelope newEnvelope = null;
            //Get Message
            _queue.TryDequeue(DateTime.Now, TimeSpan.FromSeconds(5), out newEnvelope);
            Assert.IsNotNull(newEnvelope);
            //Wait 5 seconds and reget same message
            MessageEnvelope timeoutEnvelope = null;
            _queue.TryDequeue(DateTime.Now.AddSeconds(5), TimeSpan.FromSeconds(5), out timeoutEnvelope);

            Assert.IsNotNull(timeoutEnvelope);
            //Assert It's the same message
            Assert.AreEqual(newEnvelope.Id, timeoutEnvelope.Id);
            //Second retrieval should yield timeout
            Assert.AreEqual("Operation Timed Out", timeoutEnvelope.LastErrorMessage);
        }
       [Ignore]
        [TestMethod]
        public void ShouldRetrieveMessage()
        {
            var message = new TestMessage("This is a test");
            _queue.Push(message, _queue.LocalEndPoint);
            MessageEnvelope newEnvelope = null;
            _queue.TryDequeue(DateTime.Now, TimeSpan.FromSeconds(5), out newEnvelope);
            Assert.IsNotNull(newEnvelope);
        }
     
       // [Ignore]
       [TestMethod]
       public void ShouldNotRetrieveSameMessageTwice()
       {

           var message = new TestMessage("This is a test");
          _queue.Push(message, _queue.LocalEndPoint);
           MessageEnvelope newEnvelope = null;
           var t1 = Task.Factory.StartNew(() => _queue.TryDequeue(DateTime.Now, TimeSpan.FromSeconds(5), out newEnvelope));
           var t2 = Task.Factory.StartNew(() => _queue.TryDequeue(DateTime.Now, TimeSpan.FromSeconds(5), out newEnvelope));
           var t3 = Task.Factory.StartNew(() => _queue.TryDequeue(DateTime.Now, TimeSpan.FromSeconds(5), out newEnvelope));
           Task.WaitAll(t1, t2, t3);
           Assert.IsTrue(t1.Result && !t2.Result && !t3.Result || t2.Result && !t1.Result && !t3.Result || t3.Result && !t1.Result && !t2.Result);
       }
      
        [Ignore]
        [TestMethod]
        public void MarkAsCompleteShouldRemoveMessageFromQueue()
        {

            var message = new TestMessage("This is a test");
            _queue.Push(message, _queue.LocalEndPoint);
            MessageEnvelope newEnvelope = null;
            _queue.TryDequeue(DateTime.Now, TimeSpan.FromSeconds(5), out newEnvelope);
            Assert.IsNotNull(newEnvelope);
            _queue.SetMessageCompleted(newEnvelope.Id);
            _queue.TryDequeue(DateTime.Now, TimeSpan.FromSeconds(5), out newEnvelope);
            Assert.IsNull(newEnvelope);
        }

        [TestMethod]
        public void ShouldGetMessagesInInsertionOrder()
        {

            MessageEnvelope newEnvelope = null;
            var message1 = new TestMessage("Message 1");
            _queue.Push(message1, _queue.LocalEndPoint);
            var message2 = new TestMessage("Message 2");
            _queue.Push(message2, _queue.LocalEndPoint);
            var message3 = new TestMessage("Message 3");
            var envelope3 = new MessageEnvelope(message3, _queue.LocalEndPoint, _queue.LocalEndPoint, DateTimeOffset.UtcNow.Ticks);
            _queue.Push(message3, _queue.LocalEndPoint);
            _queue.TryDequeue(DateTime.Now, TimeSpan.FromSeconds(5), out newEnvelope);
           // Assert.AreEqual(envelope1.Id, newEnvelope.Id);
            _queue.TryDequeue(DateTime.Now, TimeSpan.FromSeconds(5), out newEnvelope);
            //Assert.AreEqual(envelope2.Id, newEnvelope.Id);
            _queue.TryDequeue(DateTime.Now, TimeSpan.FromSeconds(5), out newEnvelope);
            //Assert.AreEqual(envelope3.Id, newEnvelope.Id);
        }
    }
}
