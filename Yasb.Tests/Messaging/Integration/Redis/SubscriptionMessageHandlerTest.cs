using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasb.Common.Messaging;
using Moq;
using Yasb.Redis.Messaging;
using Yasb.Common.Tests.Configuration;
using Yasb.Common.Messaging.Connections;
using Yasb.Redis.Messaging.Client.Interfaces;

namespace Yasb.Tests.Messaging.Integration.Redis
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
            var subscriptionInfo = new Mock<SubscriptionInfo<RedisConnection>>();
            subscriptionInfo.Setup(s => s.Value).Returns("myValue");
            var subscriptionInfoList = new SubscriptionInfo<RedisConnection>[] { subscriptionInfo.Object};
            var subscription = new SubscriptionMessage<RedisConnection>("myType", subscriptionInfoList);
            var client = new Mock<IRedisClient>();
            var sut = new RedisSubscriptionMessageHandler(client.Object);
            sut.Handle(subscription);
            client.Verify(s =>s.Sadd("myType","myValue"), Times.Once());
        }
    }
}
