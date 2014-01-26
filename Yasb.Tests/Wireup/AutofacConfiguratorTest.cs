using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasb.Wireup;
using Yasb.Common.Tests;
using Yasb.Common.Messaging;
using Yasb.Redis.Messaging.Configuration;

namespace Yasb.Tests.Wireup
{
    [TestClass]
    public class AutofacConfiguratorTest
    {
        private RedisConfigurator _sut =  new RedisConfigurator();

       // [Ignore]
        [TestMethod]
        public void ShouldBeAbleToCreateServiceBus()
        {
            var bus = _sut.Bus(sb => sb.EndPoints(lec => lec.ReceivesOn(c => c.WithHostName("192.168.227.128").WithQueueName("redis_producer"))).ConfigureSubscriptionService(cfg => cfg.WithHostName("192.168.227.128")));
                                     
            Assert.IsNotNull(bus);
        }
       
    }
}
