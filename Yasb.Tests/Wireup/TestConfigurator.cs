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
        public Mock<ISubscriptionService> SubscriptionService { get;  set; }
        protected override void RegisterQueueModule(QueueConfiguration<TestConnection> queueConfiguration)
        {
            QueueFactory = new TestQueueFactory(queueConfiguration);
            Builder.RegisterModule(new TestQueueModule(QueueFactory,queueConfiguration, "queue"));
        }

        protected override void RegisterServiceBusModule(ServiceBusConfiguration<TestConnection> serviceBusConfiguration)
        {
            QueueFactory = new TestQueueFactory(serviceBusConfiguration);
            Builder.RegisterModule(new TestQueueModule(QueueFactory,serviceBusConfiguration, "bus"));
            Builder.RegisterModule(new TestServiceBusModule(SubscriptionService,serviceBusConfiguration));
        }
    }
}
