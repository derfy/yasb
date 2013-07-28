using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasb.Common.Messaging;
using Moq;
using Yasb.Redis.Messaging;
using Yasb.Common.Tests.Configuration;

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
            var endPoint = new Mock<QueueEndPoint<TestConnection>>();
            var subscription = new Mock<SubscriptionMessage<TestConnection>>();
            subscription.Setup(s => s.TypeName).Returns("myType");
            var subscriptionService = new Mock<ISubscriptionService<TestConnection>>();
            var sut = new SubscriptionMessageHandler<TestConnection>(subscriptionService.Object);
            sut.Handle(subscription.Object);
            //subscriptionService.Verify(s => s.Handle(subscription.Object), Times.Once());
        }
    }
}
