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
using Yasb.Msmq.Messaging.Configuration;

namespace Yasb.Tests.Messaging.Msmq
{
    /// <summary>
    /// Summary description for MsmqQueueTest
    /// </summary>
    [TestClass]
    public class MsmqQueueTest
    {
        [Ignore]
        [TestMethod]
        public void TestMethod1()
        {
            var sut = new MsmqConfigurator().ConfigureQueue(e=>e.WithEndPoint("localConnection","test_msmq_local","queue1")
                .ConfigureConnections<MsmqFluentConnectionConfigurer>(c => c.WithConnection("localConnection", "localhost"))).CreateFromEndPointName("queue1");
            var localEndPoint = @".\Private$\test_msmq_local";
            var remoteEndPoint = @".\Private$\test_msmq_remote";
            var message = new TestMessage();
            var messageEnvelope = new MessageEnvelope(message,  localEndPoint, remoteEndPoint);
           sut.Push(messageEnvelope);

            sut.TryDequeue(DateTime.Now, new TimeSpan(0, 0, 50), out messageEnvelope);
            Assert.IsNotNull(messageEnvelope);
           // sut.SetMessageCompleted(messageEnvelope.Id);
           // sut.TryGetEnvelope(DateTime.Now, new TimeSpan(0, 0, 50), out messageEnvelope);
           // Assert.IsNull(messageEnvelope);

        }
    }
}
