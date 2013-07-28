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
    public class TestConfigurator : AbstractConfigurator<TestConnection>
    {
        public TestQueueFactory QueueFactory { get; private set; }
        public Mock<ISubscriptionService<TestConnection>> SubscriptionService { get; set; }
        protected override void RegisterQueueModule(ContainerBuilder builder, QueueConfiguration<TestConnection> queueConfiguration)
        {
            QueueFactory = new TestQueueFactory(queueConfiguration);
            builder.RegisterModule(new TestQueueModule(QueueFactory,queueConfiguration, "queue"));
        }

        protected override void RegisterServiceBusModule(ContainerBuilder builder, ServiceBusConfiguration<TestConnection> serviceBusConfiguration)
        {
            QueueFactory = new TestQueueFactory(serviceBusConfiguration);
            builder.RegisterModule(new TestQueueModule(QueueFactory,serviceBusConfiguration, "bus"));
            builder.RegisterModule(new TestServiceBusModule(SubscriptionService,serviceBusConfiguration));
        }
    }
}
