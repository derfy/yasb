using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasb.Common.Messaging;
using Moq;

namespace Yasb.Tests.Common.Messaging
{
    /// <summary>
    /// Summary description for MessagesSenderTest
    /// </summary>
    [TestClass]
    public class MessagesSenderTest
    {
        

        [TestMethod]
        public void ShouldInvokeQueuePushOnSend()
        {
            var endPoint=new BusEndPoint("foo",88,"bar");
            var queue=new Mock<IQueue>();
            var endpointName = "foo";
           Func<string,IQueue> func = name => queue.Object;
           var sut = new MessagesSender(func);
           var envelope = new MessageEnvelope(new FooMessage(), Guid.NewGuid(), endPoint, endPoint);
           sut.Send(endpointName, envelope);
           queue.Verify(q => q.Push(envelope));

        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SendShouldThrowWhenEndpointIsNull()
        {
            var endPoint = new BusEndPoint("foo", 88, "bar");
            var queue = new Mock<IQueue>();
            Func<string, IQueue> func = endpoint => queue.Object;
            var sut = new MessagesSender(func);
            var envelope = new MessageEnvelope(new FooMessage(), Guid.NewGuid(), endPoint, endPoint);
            sut.Send(null, envelope);

        }
    }
}
