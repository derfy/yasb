using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasb.Msmq.Messaging;
using Yasb.Common.Messaging;
using Moq;
using Yasb.Common.Serialization;
using Newtonsoft.Json;
using Yasb.Msmq.Messaging.Serialization;
using System.Messaging;
using Yasb.Tests.Common.Serialization;
using Yasb.Wireup;
using Yasb.Common.Tests;
using System.Threading.Tasks;
using Yasb.Common.Messaging.Configuration.Msmq;

namespace Yasb.Tests.Messaging.Msmq
{
    /// <summary>
    /// Summary description for MsmqQueueTest
    /// </summary>
    [TestClass]
    public class MsmqQueueTest
    {
        private IQueue _queue;
        public MsmqQueueTest()
        {
            _queue = new MsmqConfigurator().ConfigureQueue(e => e.WithEndPoint("localConnection", "test_msmq", "myEndPoint")
                .ConfigureConnections<MsmqFluentConnectionConfigurer>(c => c.WithConnection("localConnection", "localhost")))
                .CreateFromEndPointName("myEndPoint");
           
            
        }

        [TestInitialize()]
        public void BeforeTest()
        {
            var queue = _queue as MsmqQueue;
            queue.Clear();
            var message = new TestMessage();
            MessageEnvelope envelope = new MessageEnvelope(message, _queue.LocalEndPoint, _queue.LocalEndPoint);
            queue.Push(envelope);
        }

        [Ignore]
        [TestMethod]
        public void ShouldSetTimeoutError()
        {


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


            MessageEnvelope newEnvelope = null;
            _queue.TryDequeue(DateTime.Now, TimeSpan.FromSeconds(5), out newEnvelope);
            Assert.IsNotNull(newEnvelope);
            Assert.AreNotEqual(newEnvelope.Id,Guid.Empty);
        }
        [Ignore]
        [TestMethod]
        public void ShouldNotRetrieveSameMessageTwice()
        {


            MessageEnvelope newEnvelope = null;
            var t1=Task.Factory.StartNew(()=>_queue.TryDequeue(DateTime.Now, TimeSpan.FromSeconds(5), out newEnvelope));
            var t2=Task.Factory.StartNew(() => _queue.TryDequeue(DateTime.Now, TimeSpan.FromSeconds(5), out newEnvelope));
            var t3=Task.Factory.StartNew(() => _queue.TryDequeue(DateTime.Now, TimeSpan.FromSeconds(5), out newEnvelope));
            Task.WaitAll(t1, t2, t3);
            Assert.IsTrue(t1.Result && !t2.Result && !t3.Result || t2.Result && !t1.Result && !t3.Result || t3.Result && !t1.Result && !t2.Result);
        }
       
        [Ignore]
        [TestMethod]
        public void MarkAsCompleteShouldRemoveMessageFromQueue()
        {


            MessageEnvelope newEnvelope = null;
            _queue.TryDequeue(DateTime.Now, TimeSpan.FromSeconds(5), out newEnvelope);
            Assert.IsNotNull(newEnvelope);
            _queue.SetMessageCompleted(newEnvelope.Id);
            _queue.TryDequeue(DateTime.Now, TimeSpan.FromSeconds(5), out newEnvelope);
            Assert.IsNull(newEnvelope);
        }


    
    }
}
