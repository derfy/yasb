using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Tests.Configuration;
using Yasb.Common.Messaging.Configuration;
using Autofac;
using Yasb.Common.Messaging;
using Yasb.Wireup;
using Moq;
using Yasb.Redis.Messaging.Configuration;

namespace Yasb.Tests.Wireup
{

    public class TestQueueModule : ScopedModule<ServiceBusConfiguration<TestEndPoint, RedisEndPointConfiguration>>
    {
        private IQueueFactory<TestEndPoint> _queueFactory;
        public TestQueueModule(IQueueFactory<TestEndPoint> queueFactory, ServiceBusConfiguration<TestEndPoint, RedisEndPointConfiguration> queueConfiguration, string scope)
            : base(queueConfiguration,scope)
        {
             _queueFactory = queueFactory;
        }
        
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterWithScope<IQueueFactory<TestEndPoint>>((componentScope, parameters) =>
            {
                return _queueFactory;
            }).InstancePerMatchingLifetimeScope(Scope);
            
            
        }
    }

    public class TestServiceBusModule : ServiceBusModule<TestEndPointConfiguration, RedisEndPointConfiguration>
    {
        Mock<ISubscriptionService<TestEndPointConfiguration>> _subscriptionService;
        Mock<IQueueFactory<TestEndPointConfiguration>> _queueFactory;
        public TestServiceBusModule(ServiceBusConfiguration<TestEndPointConfiguration, RedisEndPointConfiguration> configuration, Mock<IQueueFactory<TestEndPointConfiguration>> queueFactory, Mock<ISubscriptionService<TestEndPointConfiguration>> subscriptionService)
            : base(configuration)
        {
            _queueFactory = queueFactory;
             _subscriptionService = subscriptionService;
        }
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterWithScope<IQueueFactory<TestEndPointConfiguration>>((componentScope, parameters) =>
            {
                return _queueFactory.Object;
            }).InstancePerMatchingLifetimeScope(Scope);
            builder.RegisterWithScope<ISubscriptionService<TestEndPointConfiguration>>((componentScope, parameters) =>
            {

                return _subscriptionService.Object;
            });
        }
    }
}
