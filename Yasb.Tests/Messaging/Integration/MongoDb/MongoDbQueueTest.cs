using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasb.Common.Messaging;
using Yasb.Wireup;
using Yasb.Common.Messaging.Configuration.MongoDb;
using Yasb.MongoDb.Messaging;
using Yasb.Common.Tests;
using System.Threading.Tasks;

namespace Yasb.Tests.Messaging.Integration.MongoDb
{
    /// <summary>
    /// Summary description for MongoDbQueueTest
    /// </summary>
    [TestClass]
    public class MongoDbQueueTest
    {
        private IQueue<MongoDbConnection> _queue;
        public MongoDbQueueTest()
        {
            _queue = new MongoDbConfigurator().ConfigureQueue(e => e.WithEndPoint("vmEndPoint", "test_mongodb", "myEndPoint")
                .ConfigureConnections<MongoDbFluentConnectionConfigurer>(c => c.WithConnection("vmEndPoint", "192.168.127.128", "test")))
                .CreateFromEndPointName("myEndPoint");

            
           
        }

        //[TestInitialize()]
        //public void BeforeTest()
        //{
        //    var queue = _queue as MongoDbQueue;
        //    queue.Clear();


        //}


        //[TestMethod]
        //public void ShouldRetrieveMessage()
        //{

        //    var message = new TestMessage("This is a test");
        //    var envelope = new MessageEnvelope(message, _queue.LocalEndPoint, _queue.LocalEndPoint, DateTimeOffset.UtcNow.Ticks);
        //    _queue.Push(envelope);
        //    MessageEnvelope newEnvelope = null;
        //    _queue.TryDequeue(DateTime.Now, TimeSpan.FromSeconds(5), out newEnvelope);
        //    Assert.IsNotNull(newEnvelope);
        //    Assert.AreNotEqual(newEnvelope.Id, Guid.Empty);
        //}

        //[TestMethod]
        //public void ShouldNotRetrieveSameMessageTwice()
        //{

        //    var message = new TestMessage("This is a test");
        //    var envelope = new MessageEnvelope(message, _queue.LocalEndPoint, _queue.LocalEndPoint, DateTimeOffset.UtcNow.Ticks);
        //    _queue.Push(envelope);
        //    MessageEnvelope newEnvelope = null;
        //    var t1 = Task.Factory.StartNew(() => _queue.TryDequeue(DateTime.Now, TimeSpan.FromSeconds(5), out newEnvelope));
        //    var t2 = Task.Factory.StartNew(() => _queue.TryDequeue(DateTime.Now, TimeSpan.FromSeconds(5), out newEnvelope));
        //    var t3 = Task.Factory.StartNew(() => _queue.TryDequeue(DateTime.Now, TimeSpan.FromSeconds(5), out newEnvelope));
        //    Task.WaitAll(t1, t2, t3);
        //    Assert.IsTrue(t1.Result && !t2.Result && !t3.Result || t2.Result && !t1.Result && !t3.Result || t3.Result && !t1.Result && !t2.Result);
        //}


        //[Ignore]
        //[TestMethod]
        //public void MarkAsCompleteShouldRemoveMessageFromQueue()
        //{

        //    var message = new TestMessage("This is a test");
        //    var envelope = new MessageEnvelope(message, _queue.LocalEndPoint, _queue.LocalEndPoint, DateTimeOffset.UtcNow.Ticks);
        //    _queue.Push(envelope);
        //    MessageEnvelope newEnvelope = null;
        //    _queue.TryDequeue(DateTime.Now, TimeSpan.FromSeconds(5), out newEnvelope);
        //    Assert.IsNotNull(newEnvelope);
        //    _queue.SetMessageCompleted(newEnvelope.Id);
        //    _queue.TryDequeue(DateTime.Now, TimeSpan.FromSeconds(5), out newEnvelope);
        //    Assert.IsNull(newEnvelope);
        //}
        //[Ignore]
        //[TestMethod]
        //public void ShouldGetMessagesInInsertionOrder()
        //{

        //    MessageEnvelope newEnvelope = null;
        //    var message1 = new TestMessage("Message 1");
        //    var envelope1 = new MessageEnvelope(message1, _queue.LocalEndPoint, _queue.LocalEndPoint, DateTimeOffset.UtcNow.Ticks);
        //    _queue.Push(envelope1);
        //    var message2 = new TestMessage("Message 2");
        //    var envelope2 = new MessageEnvelope(message2, _queue.LocalEndPoint, _queue.LocalEndPoint, DateTimeOffset.UtcNow.Ticks);
        //    _queue.Push(envelope2);
        //    var message3 = new TestMessage("Message 3");
        //    var envelope3 = new MessageEnvelope(message3, _queue.LocalEndPoint, _queue.LocalEndPoint, DateTimeOffset.UtcNow.Ticks);
        //    _queue.Push(envelope3);
        //    _queue.TryDequeue(DateTime.Now, TimeSpan.FromSeconds(5), out newEnvelope);
        //    Assert.AreEqual(envelope1.Id, newEnvelope.Id);
        //    _queue.TryDequeue(DateTime.Now, TimeSpan.FromSeconds(5), out newEnvelope);
        //    Assert.AreEqual(envelope2.Id, newEnvelope.Id);
        //    _queue.TryDequeue(DateTime.Now, TimeSpan.FromSeconds(5), out newEnvelope);
        //    Assert.AreEqual(envelope3.Id, newEnvelope.Id);
        //}
    }
}
