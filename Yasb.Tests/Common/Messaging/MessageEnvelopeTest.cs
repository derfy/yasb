using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasb.Common.Messaging;
using Yasb.Redis.Messaging;
using Yasb.Tests.Common.Serialization;
using Yasb.Common.Tests.Configuration;
using Yasb.Common.Tests;

namespace Yasb.Tests.Common.Messaging
{
    /// <summary>
    /// Summary description for MessageEnvelopeTest
    /// </summary>
    [TestClass]
    public class MessageEnvelopeTest
    {
        
        [TestMethod]
        public void ShouldGetCorrectTypeName()
        {
            var message=new TestMessage();
            var fromEndPoint = new BusEndPoint("myConnection:myQueue");
            var toEndPoint = new BusEndPoint("myConnection:myQueue");
            var envelope = new MessageEnvelope(message,  fromEndPoint, toEndPoint);
            Assert.AreEqual(typeof(TestMessage), envelope.ContentType);
        }
    }
}
