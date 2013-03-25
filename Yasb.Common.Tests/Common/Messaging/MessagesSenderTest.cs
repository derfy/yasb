using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasb.Common.Messaging;
using Moq;
using Yasb.Redis.Messaging;

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
            var endPoint = new TestEndPoint("myTestAddress:80:myQueue");
            var queue=new Mock<IQueue>();
            
            Func<IEndPoint, IQueue> func = e => queue.Object;
           var sut = new MessagesSender<TestEndPoint>(func);
           var envelope = new MessageEnvelope(new FooMessage(), Guid.NewGuid(), endPoint, endPoint);
           sut.Send(endPoint, envelope);
           queue.Verify(q => q.Push(envelope));

        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SendShouldThrowWhenEndpointIsNull()
        {
            var endPoint = new TestEndPoint("myTestAddress:80:myQueue");
            var queue = new Mock<IQueue>();
            Func<IEndPoint, IQueue> func = endpoint => queue.Object;
            var sut = new MessagesSender<TestEndPoint>(func);
            var envelope = new MessageEnvelope(new FooMessage(), Guid.NewGuid(), endPoint, endPoint);
            sut.Send(null, envelope);

        }
    }
}
