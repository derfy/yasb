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

namespace Yasb.Tests.Wireup
{
    public class TestConfigurator : AbstractConfigurator<TestEndPoint, TestSerializationConfiguration>
    {
        private Mock<IQueueFactory<TestEndPoint>> _queueFactory;
        public TestConfigurator(Mock<IQueueFactory<TestEndPoint>> queueFactory)
        {
             _queueFactory = queueFactory;
        }
        public Mock<ISubscriptionService<TestEndPoint>> SubscriptionService { get; set; }
        protected override void RegisterQueueModule(ContainerBuilder builder, ServiceBusConfiguration<TestEndPoint, TestSerializationConfiguration> queueConfiguration)
        {
            // builder.RegisterModule(new TestQueueModule(_queueFactory.Object, queueConfiguration, "queue"));
        }

        protected override void RegisterServiceBusModule(ContainerBuilder builder, ServiceBusConfiguration<TestEndPoint,  TestSerializationConfiguration> serviceBusConfiguration)
        {
           // builder.RegisterModule(new TestQueueModule(_endPointFactory.Object, _queueFactory.Object, serviceBusConfiguration, "bus"));
            builder.RegisterModule(new TestServiceBusModule(serviceBusConfiguration, _queueFactory, SubscriptionService));
        }
    }
}
