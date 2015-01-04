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
using Autofac;
using System.Net;
using Yasb.Redis.Messaging.Client.Interfaces;
using System.Threading.Tasks;
using Yasb.Common.Messaging.Configuration;
using Yasb.Redis.Messaging.Configuration;
using Yasb.Common.Messaging.Serialization.Json;
using Moq;
using Newtonsoft.Json;
using Yasb.Redis.Messaging.Serialization.MessageDeserializers;
using Yasb.Common.Messaging.Tcp;

namespace Yasb.Tests.Messaging.Redis
{
    public class TestMessageHandler : IHandleMessages
    {
        public void Handle<TMessage>(TMessage message)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// Summary description for RedisQueueTest
    /// </summary>
    [TestClass]
    public class RedisQueueTest
    {
        private IQueue<RedisEndPoint> _queue;
        private AbstractJsonSerializer<MessageEnvelope> _serializer = new JsonMessageEnvelopeSerializer(type => new DefaultJsonMessageDeserializer(type));
            //new Mock<AbstractJsonSerializer<MessageEnvelope>>();
        public RedisQueueTest()
        {
            var localEndPoint=new RedisEndPointConfiguration("192.168.227.128","queue_test");
            var connectionPool = new TcpConnectionsPool<RedisConnection>(10, () => new RedisConnection(localEndPoint.Built.Address));
            var connectionManager = new TcpConnectionsManager<RedisConnection>(connectionPool);
            var redisClient = new RedisClient(connectionManager);
            _queue = new RedisQueue(localEndPoint.Built, _serializer, redisClient);
        }
        [TestInitialize()]
        public void BeforeTest()
        {
            _queue.Clear();
            
        }
      //  [Ignore]
        [TestMethod]
        public void ShouldSetTimeoutError()
        {
            var message = new TestMessage("This is a test");
            var envelope = new MessageEnvelope(message);
            _queue.Push(message);
            MessageEnvelope newEnvelope = null;
            //Get Message
            _queue.TryDequeue(DateTime.Now, TimeSpan.FromSeconds(5), out newEnvelope);
            Assert.IsNotNull(newEnvelope);
            //Assert.AreEqual(newEnvelope.From.Value,_fromEndPoint.Value);
            //Wait 5 seconds and reget same message
            MessageEnvelope timeoutEnvelope = null;
            _queue.TryDequeue(DateTime.Now.AddSeconds(5), TimeSpan.FromSeconds(5), out timeoutEnvelope);

            Assert.IsNotNull(timeoutEnvelope);
            //Assert It's the same message
            Assert.AreEqual(newEnvelope.Id, timeoutEnvelope.Id);
            //Second retrieval should yield timeout
            Assert.AreEqual("Operation Timed Out", timeoutEnvelope.LastErrorMessage);
        }
        [TestMethod]
        public void ShouldRetrieveMessage()
        {
            var message = new TestMessage("This is a test");
            var envelope = new MessageEnvelope(message);
            _queue.Push(message);

            MessageEnvelope newEnvelope = null;
            _queue.TryDequeue(DateTime.Now, TimeSpan.FromSeconds(5), out newEnvelope);
            Assert.IsNotNull(newEnvelope);
        }
     
       // [Ignore]
       [TestMethod]
       public void ShouldNotRetrieveSameMessageTwice()
       {

           var message = new TestMessage("This is a test");
           var envelope = new MessageEnvelope( message);
           _queue.Push(message);
           MessageEnvelope newEnvelope = null;
           var t1 = Task.Factory.StartNew(() => _queue.TryDequeue(DateTime.Now, TimeSpan.FromSeconds(5), out newEnvelope));
           var t2 = Task.Factory.StartNew(() => _queue.TryDequeue(DateTime.Now, TimeSpan.FromSeconds(5), out newEnvelope));
           var t3 = Task.Factory.StartNew(() => _queue.TryDequeue(DateTime.Now, TimeSpan.FromSeconds(5), out newEnvelope));
           Task.WaitAll(t1, t2, t3);
           Assert.IsTrue(t1.Result && !t2.Result && !t3.Result || t2.Result && !t1.Result && !t3.Result || t3.Result && !t1.Result && !t2.Result);
       }
      
        [TestMethod]
        public void MarkAsCompleteShouldRemoveMessageFromQueue()
        {

            var message = new TestMessage("This is a test");

            var envelope = new MessageEnvelope( message);
            _queue.Push(message);
            MessageEnvelope newEnvelope = null;
            _queue.TryDequeue(DateTime.Now, TimeSpan.FromSeconds(5), out newEnvelope);
            Assert.IsNotNull(newEnvelope);
            _queue.SetMessageCompleted(newEnvelope.Id, DateTime.Now);
            _queue.TryDequeue(DateTime.Now, TimeSpan.FromSeconds(5), out newEnvelope);
            Assert.IsNull(newEnvelope);
        }

        [TestMethod]
        public void ShouldGetMessagesInInsertionOrder()
        {

            MessageEnvelope newEnvelope = null;
            var message1 = new TestMessage("Message 1");
            var envelope1 = new MessageEnvelope(message1);
            _queue.Push(message1);
            var message2 = new TestMessage("Message 2");
            var envelope2 = new MessageEnvelope(message2);
            _queue.Push(message2);
            var message3 = new TestMessage("Message 3");
            var envelope3 = new MessageEnvelope( message3);
            _queue.Push(message3);
            _queue.TryDequeue(DateTime.Now, TimeSpan.FromSeconds(5), out newEnvelope);
            Assert.AreEqual(message1.Value, ((TestMessage)newEnvelope.Message).Value);
            _queue.TryDequeue(DateTime.Now, TimeSpan.FromSeconds(5), out newEnvelope);
            Assert.AreEqual(message2.Value, ((TestMessage)newEnvelope.Message).Value);
            _queue.TryDequeue(DateTime.Now, TimeSpan.FromSeconds(5), out newEnvelope);
            Assert.AreEqual(message3.Value, ((TestMessage)newEnvelope.Message).Value);
        }
        private static string CreateGuid() {
            return Guid.NewGuid().ToString();
        }
    }
}
