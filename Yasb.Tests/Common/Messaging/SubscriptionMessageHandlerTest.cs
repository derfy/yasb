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
            var subscription = new Mock<SubscriptionMessage>();
            subscription.Setup(s => s.SubscriberEndPoint).Returns("myId");
            subscription.Setup(s => s.TypeName).Returns("myType");
            var subscriptionService = new Mock<ISubscriptionService>();
            var sut = new SubscriptionMessageHandler(subscriptionService.Object);
            sut.Handle(subscription.Object);
            subscriptionService.Verify(s => s.Subscribe("myType", "myId"), Times.Once());
        }
    }
}
