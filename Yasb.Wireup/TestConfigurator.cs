using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging.Configuration;
using Autofac;
using Yasb.Common.Messaging;
using Yasb.Common.Tests.Configuration;
using Moq;

namespace Yasb.Wireup
{
    public class TestResolver : IResolver<TestResolver>
    {

        private ILifetimeScope _lifetimeScope;
        private EndPointConfiguration _endPointConfiguration;
        public TestResolver(ILifetimeScope lifetimeScope, EndPointConfiguration endPointConfiguration)
        {
            _endPointConfiguration = endPointConfiguration;
            _lifetimeScope = lifetimeScope;
        }
        public IServiceBus Bus()
        {
            return _lifetimeScope.Resolve<IServiceBus>();
        }
        public IQueue GetLocalQueue()
        {
            var factory = _lifetimeScope.Resolve<QueueFactory>();
            return factory(_endPointConfiguration.LocalEndPoint);
        }
        public IQueue GetQueueByName(string endPointName)
        {
            var endPoint = _endPointConfiguration.NamedEndPoints.Where(e => e.Name == endPointName).FirstOrDefault();
            if (endPoint == null)
                throw new ApplicationException(string.Format("No endPoint with name {0}", endPointName));
            var factory = _lifetimeScope.Resolve<QueueFactory>();
            return factory(endPoint);
        }
        
    }

    public class TestConfigurator : IConfigurator<TestResolver, TestConnectionConfiguration> 
    {
        protected ContainerBuilder Builder{get;private set;}
        Mock<ISubscriptionService> _subscriptionService;
        private Mock<IQueue> _queue;
        Dictionary<BusEndPoint, Mock<IQueue>> _dict;
        public TestConfigurator(Dictionary<BusEndPoint, Mock<IQueue>> dict, Mock<ISubscriptionService> subscriptionService)
        {
            _subscriptionService = subscriptionService;
            _dict = dict;
            Builder = new ContainerBuilder();
        }

        

        public IConfigurator<TestResolver, TestConnectionConfiguration> ConfigureServiceBus(Action<ServiceBusConfigurer<TestConnectionConfiguration>> busConfigurer)
        {
            var serviceBusConfigurer = new ServiceBusConfigurer<TestConnectionConfiguration>();
            busConfigurer(serviceBusConfigurer);
            Builder.RegisterModule(new CommonModule(serviceBusConfigurer.Built));
            Builder.RegisterModule(new TestModule(_dict, _subscriptionService));


            Builder.Register<TestResolver>(c =>
            {
                var lifetimeScope = c.Resolve<ILifetimeScope>();
                return new TestResolver(c.Resolve<ILifetimeScope>().BeginLifetimeScope("bus"), serviceBusConfigurer.Built.EndPointConfiguration);
            });
            
            return this;
        }

        public TestResolver Configure()
        {
            return Builder.Build().Resolve<TestResolver>();
        }
    }
}
