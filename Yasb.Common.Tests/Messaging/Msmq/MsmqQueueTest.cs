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

namespace Yasb.Tests.Messaging.Msmq
{
    /// <summary>
    /// Summary description for MsmqQueueTest
    /// </summary>
    [TestClass]
    public class MsmqQueueTest
    {
        
       // [Ignore]
        [TestMethod]
        public void TestMethod1()
        {
           // var localEndPoint = new MsmqEndPoint("test_msmq_local@localhost");
           // var remoteEndPoint = new MsmqEndPoint("test_msmq_remote@localhost");
           // var message = new TestMessage();
           // var messageEnvelope = new MessageEnvelope(message,  localEndPoint, remoteEndPoint);
           //var converters = new List<JsonConverter>() {  new MessageEnvelopeConverter<MsmqEndPoint>() }.ToArray();
           // var serializer = new Serializer(converters);
           // var formatter = new JsonMessageFormatter<MessageEnvelope>(serializer);
           // var sut = new MsmqQueue(localEndPoint, formatter);
           // sut.Initialize();
           // sut.Push(messageEnvelope);

           // sut.TryGetEnvelope(DateTime.Now, new TimeSpan(0, 0, 50), out messageEnvelope);
           // Assert.IsNotNull(messageEnvelope);
           // sut.SetMessageCompleted(messageEnvelope.Id);
           // sut.TryGetEnvelope(DateTime.Now, new TimeSpan(0, 0, 50), out messageEnvelope);
           // Assert.IsNull(messageEnvelope);

        }
    }
}
