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
    /// Summary description for SubscriptionMessageHandlerTest
    /// </summary>
    [TestClass]
    public class SubscriptionMessageHandlerTest
    {
       

        [TestMethod]
        public void ShouldInvokeSubscriptionServiceAddOnHandle()
        {
            var subscription = new SubscriptionMessage<TestEndPoint>() { TypeName = "foo" };
            var subscriptionService = new Mock<ISubscriptionService>();
            var sut = new SubscriptionMessageHandler<TestEndPoint>(subscriptionService.Object);
            sut.Handle(subscription);
            subscriptionService.Verify(s => s.AddSubscriberFor("foo", It.IsAny<RedisEndPoint>()), Times.Once());
        }
    }
}
