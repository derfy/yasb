using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasb.Wireup;
using Yasb.Common.Tests;
using Yasb.Common.Messaging;
using Yasb.Common.Messaging.EndPoints.Redis;
using Yasb.Common.Messaging.Configuration.Redis;

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
            var serviceBus = _sut.Bus(c => c.EndPoints<RedisEndPointConfiguration>(e => e.ReceivesOn(ec => ec.WithQueueName("localQueue"))));
                                           //.ConfigureConnections<FluentIPEndPointConfigurer>(conn => conn.WithConnection("local", "127.0.0.1")
                                           //                                         .WithConnection("myHost", "127.0.0.1")
                                           //                                         .WithConnection("myOtherHost", "192.198.70.86"))
            Assert.IsNotNull(serviceBus);
        }
      //  [Ignore]
        [TestMethod]
        public void ShouldBeAbleToCreateQueue()
        {
            //var queue = _sut.ConfigureQueue(c => c.WithLocalEndPoint("myHost", "queue_test"));
                //.ConfigureConnections<FluentIPEndPointConfigurer>(conn => conn.WithConnection("local", "127.0.0.1")
                //                                                              .WithConnection("myHost", "127.0.0.1")
                //                                                              .WithConnection("myOtherHost", "192.198.70.86")));
          //  Assert.IsNotNull(queue);
            
        }
    }
}
