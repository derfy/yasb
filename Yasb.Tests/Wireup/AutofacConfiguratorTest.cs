using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasb.Wireup;
using Yasb.Wireup.ConfiguratorExtensions.Redis;
using Yasb.Common.Tests;
using Yasb.Common.Messaging;
using Yasb.Redis.Messaging.Configuration;
using Yasb.Redis.Messaging;

namespace Yasb.Tests.Wireup
{
    [TestClass]
    public class AutofacConfiguratorTest
    {
       
       // [Ignore]
        [TestMethod]
        public void ShouldBeAbleToCreateServiceBus()
        {
            var bus = Configurator.Configure<RedisEndPoint>().ConfigureEndPoints(lec => lec.ReceivesOn(c => c.WithHostName("192.168.227.128").WithQueueName("redis_producer")))
                .ConfigureSubscriptionService(cfg => cfg.WithHostName("192.168.227.128")).Bus();
                                     
            Assert.IsNotNull(bus);
        }
       
    }
}
