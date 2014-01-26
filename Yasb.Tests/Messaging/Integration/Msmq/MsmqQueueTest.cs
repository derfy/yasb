using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasb.Msmq.Messaging;
using Yasb.Common.Messaging;
using Moq;
using Newtonsoft.Json;
using System.Messaging;
using Yasb.Tests.Common.Serialization;
using Yasb.Wireup;
using Yasb.Common.Tests;
using System.Threading.Tasks;
using System.Net;
using Yasb.Tests.Messaging.Redis;
using Yasb.Common.Messaging.Serialization.Xml;
using System.IO;
using System.Xml;
using Yasb.Msmq.Messaging.Configuration;

namespace Yasb.Tests.Messaging.Msmq
{
    /// <summary>
    /// Summary description for MsmqQueueTest
    /// </summary>
    [TestClass]
    public class MsmqQueueTest
    {
        private IQueue<MsmqEndPointConfiguration> _queue;
       private Mock<AbstractXmlSerializer<MessageEnvelope>> serializer = new Mock<AbstractXmlSerializer<MessageEnvelope>>();
        public MsmqQueueTest()
        {
            var localEndPoint=new MsmqEndPointConfiguration(".","test");

            
            _queue = new MsmqQueue(localEndPoint, serializer.Object);
            
        }

        [TestInitialize()]
        public void BeforeTest()
        {
            _queue.Clear();
          
        }

        //[Ignore]
        [TestMethod]
        public void ShouldSetTimeoutError()
        {

            var message = new TestMessage("This is a test");
           MessageEnvelope envelope = new MessageEnvelope(message);
            _queue.Push(message);
            serializer.Setup(s => s.Deserialize(It.IsAny<XmlReader>()))
               .Returns(new MessageEnvelope() { RetriesNumber = 2, Id = "abc", Message = message });
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
       
       // [Ignore]
        [TestMethod]
        public void ShouldRetrieveMessage()
        {
            var message = new TestMessage("This is a test");
           MessageEnvelope envelope = new MessageEnvelope(message);
            _queue.Push(message);
            serializer.Setup(s => s.Deserialize(It.IsAny<XmlReader>()))
               .Returns(new MessageEnvelope() { RetriesNumber = 2, Id = "abc", Message = message });
           MessageEnvelope newEnvelope = null;
            _queue.TryDequeue(DateTime.Now, TimeSpan.FromSeconds(5), out newEnvelope);
            Assert.IsNotNull(newEnvelope);
            Assert.AreNotEqual(newEnvelope.Id,Guid.Empty);
        }
       // [Ignore]
        [TestMethod]
        public void ShouldNotRetrieveSameMessageTwice()
        {

            var message = new TestMessage("This is a test");
           MessageEnvelope envelope = new MessageEnvelope(message);
            _queue.Push(message);
           MessageEnvelope newEnvelope = null;
           serializer.Setup(s => s.Deserialize(It.IsAny<XmlReader>()))
              .Returns(new MessageEnvelope() { RetriesNumber = 2, Id = "abc", Message = message });
            var t1=Task.Factory.StartNew(()=>_queue.TryDequeue(DateTime.Now, TimeSpan.FromSeconds(5), out newEnvelope));
            var t2=Task.Factory.StartNew(() => _queue.TryDequeue(DateTime.Now, TimeSpan.FromSeconds(5), out newEnvelope));
            var t3=Task.Factory.StartNew(() => _queue.TryDequeue(DateTime.Now, TimeSpan.FromSeconds(5), out newEnvelope));
            Task.WaitAll(t1, t2, t3);
            Assert.IsTrue(t1.Result && !t2.Result && !t3.Result || t2.Result && !t1.Result && !t3.Result || t3.Result && !t1.Result && !t2.Result);
        }
       
      //  [Ignore]
        [TestMethod]
        public void MarkAsCompleteShouldRemoveMessageFromQueue()
        {
            
            var message = new TestMessage("This is a test");
           MessageEnvelope envelope = new MessageEnvelope(message);
            _queue.Push(message);
           MessageEnvelope newEnvelope = null;
           serializer.Setup(s => s.Deserialize(It.IsAny<XmlReader>()))
              .Returns(new MessageEnvelope() { RetriesNumber = 2, Id = "abc", Message = message });
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
            _queue.Push(message1);

            var message2 = new TestMessage("Message 2");
            _queue.Push(message2);

            var message3 = new TestMessage("Message 3");
            _queue.Push(message3);
            serializer.Setup(s => s.Deserialize(It.IsAny<XmlReader>()))
                .Returns(new MessageEnvelope() { RetriesNumber = 2, Id = "abc" ,Message=message1});
            _queue.TryDequeue(DateTime.Now, TimeSpan.FromSeconds(5), out newEnvelope);
            Assert.AreEqual(message1, newEnvelope.Message);

            serializer.Setup(s => s.Deserialize(It.IsAny<XmlReader>()))
               .Returns(new MessageEnvelope() { RetriesNumber = 2, Id = "abc", Message = message2 });
            _queue.TryDequeue(DateTime.Now, TimeSpan.FromSeconds(5), out newEnvelope);
            Assert.AreEqual(message2, newEnvelope.Message);

            serializer.Setup(s => s.Deserialize(It.IsAny<XmlReader>()))
               .Returns(new MessageEnvelope() { RetriesNumber = 2, Id = "abc", Message = message3 });
            _queue.TryDequeue(DateTime.Now, TimeSpan.FromSeconds(5), out newEnvelope);
            Assert.AreEqual(message3, newEnvelope.Message);
        }
    
    }
}
