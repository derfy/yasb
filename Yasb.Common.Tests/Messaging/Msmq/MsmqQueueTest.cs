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
        

        [TestMethod]
        public void TestMethod1()
        {
            var localEndPoint = new MsmqEndPoint("test_msmq_local@localhost");
            var remoteEndPoint = new MsmqEndPoint("test_msmq_remote@localhost");
            var id=string.Format(@"{0}\{1}",Guid.NewGuid(),0);
            var message = new TestMessage();
            var messageEnvelope = new MessageEnvelope(message, id, localEndPoint, remoteEndPoint);
            var converters = new List<JsonConverter>() { new MsmqEndPointConverter(), new MessageEnvelopeConverter<MsmqEndPoint>() }.ToArray();
            var serializer = new Serializer(converters);
            var formatter = new JsonMessageFormatter<MessageEnvelope>(serializer);
            var sut = new MsmqQueue(localEndPoint,formatter);
            sut.Initialize();
            sut.Push(messageEnvelope);

        }
    }
}
