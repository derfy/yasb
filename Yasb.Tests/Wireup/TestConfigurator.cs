using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging.Configuration;
using Yasb.Common.Tests.Configuration;
using Yasb.Wireup;
using Autofac;
using Yasb.Common.Messaging;
using Moq;
using Yasb.Redis.Messaging.Configuration;
using Autofac.Core;

namespace Yasb.Tests.Wireup
{
    public class TestConfigurator : AbstractConfigurator<TestEndPointConfiguration, RedisEndPointConfiguration>
    {
        private Mock<IQueueFactory<TestEndPointConfiguration>> _queueFactory;
        public TestConfigurator(Mock<IQueueFactory<TestEndPointConfiguration>> queueFactory)
        {
             _queueFactory = queueFactory;
        }
        public Mock<ISubscriptionService<TestEndPointConfiguration>> SubscriptionService { get; set; }


        protected override IModule RegisterServiceBusModule(ServiceBusConfiguration<TestEndPointConfiguration, RedisEndPointConfiguration> serviceBusConfiguration)
        {
           // builder.RegisterModule(new TestQueueModule(_endPointFactory.Object, _queueFactory.Object, serviceBusConfiguration, "bus"));
            return new TestServiceBusModule(serviceBusConfiguration, _queueFactory, SubscriptionService);
        }
    }
}
