using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasb.Wireup;
using Yasb.Common.Messaging.Configuration.CommonConnectionConfigurers;
using Yasb.Common.Tests;

namespace Yasb.Tests.Wireup
{
    [TestClass]
    public class AutofacConfiguratorTest
    {
        private RedisConfigurator _sut =  new RedisConfigurator();

        [Ignore]
        [TestMethod]
        public void ShouldBeAbleToCreateServiceBus()
        {
            var serviceBus= _sut.Bus(c => c.WithEndPointConfiguration(cfg => cfg.WithLocalEndPoint("local", "localQueue").WithEndPoint("myHost", "queue_test", "test"))
                                           .ConfigureConnections<FluentIPEndPointConfigurer>(conn => conn.WithConnection("local", "127.0.0.1")
                                                                                    .WithConnection("myHost", "127.0.0.1")
                                                                                    .WithConnection("myOtherHost", "192.198.70.86"))
                                            .WithMessageHandlersAssembly(typeof(TestMessage).Assembly));
            Assert.IsNotNull(serviceBus);
        }
        [Ignore]
        [TestMethod]
        public void ShouldBeAbleToCreateQueue()
        {
            var queue = _sut.ConfigureQueue(c => c.WithEndPoint("myHost", "queue_test", "test").ConfigureConnections<FluentIPEndPointConfigurer>(conn => conn.WithConnection("local", "127.0.0.1")
                                                                              .WithConnection("myHost", "127.0.0.1")
                                                                              .WithConnection("myOtherHost", "192.198.70.86"))).CreateFromEndPointName("test");
            Assert.IsNotNull(queue);
            
        }
    }
}
