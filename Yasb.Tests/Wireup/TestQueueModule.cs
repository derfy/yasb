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

namespace Yasb.Tests.Wireup
{

    public class TestQueueModule : ScopedModule<QueueConfiguration<TestEndPoint>>
    {
        private IQueueFactory<TestEndPoint> _queueFactory;
        public TestQueueModule(IQueueFactory<TestEndPoint> queueFactory, QueueConfiguration<TestEndPoint> queueConfiguration, string scope)
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
  
    public class TestServiceBusModule : ServiceBusModule<TestEndPoint,TestSerializationConfiguration>
    {
        Mock<ISubscriptionService<TestEndPoint>> _subscriptionService;
        Mock<IQueueFactory<TestEndPoint>> _queueFactory;
        public TestServiceBusModule(ServiceBusConfiguration<TestEndPoint,TestSerializationConfiguration> configuration, Mock<IQueueFactory<TestEndPoint>> queueFactory, Mock<ISubscriptionService<TestEndPoint>> subscriptionService)
            : base(configuration)
        {
            _queueFactory = queueFactory;
             _subscriptionService = subscriptionService;
        }
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterWithScope<IQueueFactory<TestEndPoint>>((componentScope, parameters) =>
            {
                return _queueFactory.Object;
            }).InstancePerMatchingLifetimeScope(Scope);
            builder.RegisterWithScope<ISubscriptionService<TestEndPoint>>((componentScope, parameters) =>
            {

                return _subscriptionService.Object;
            });
        }
    }
}
