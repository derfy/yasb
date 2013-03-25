using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasb.Common.Messaging;
using Yasb.Redis.Messaging;

namespace Yasb.Tests.Common.Messaging
{
    /// <summary>
    /// Summary description for MessageEnvelopeTest
    /// </summary>
    [TestClass]
    public class MessageEnvelopeTest
    {
        private class FooMessage : IMessage
        { }
        [TestMethod]
        public void ShouldGetCorrectTypeName()
        {
            var message=new FooMessage();
            var fromEndPoint = new TestEndPoint("myTestAddressFrom:80:myQueue");
            var toEndPoint = new TestEndPoint("myTestAddressTo:80:myQueue");
            var envelope = new MessageEnvelope(message, Guid.NewGuid(), fromEndPoint, toEndPoint);
            Assert.AreEqual(typeof(FooMessage), envelope.ContentType);
        }
    }
}
