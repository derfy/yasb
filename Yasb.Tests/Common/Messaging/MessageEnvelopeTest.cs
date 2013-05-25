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
using System.Net;
using Moq;

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
            var message=new TestMessage("foo");
            var fromEndPoint = "myConnection:myQueue";
            var toEndPoint = "myConnection:myQueue";
            var envelope = new MessageEnvelope(message,  fromEndPoint, toEndPoint,DateTimeOffset.UtcNow.Ticks);
            Assert.AreEqual(typeof(TestMessage), envelope.ContentType);
        }
    }
}
