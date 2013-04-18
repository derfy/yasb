using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using Yasb.Common.Messaging;
using Yasb.Common.Messaging.Configuration;

namespace Yasb.Wireup
{
    public abstract class AbstractConfigurator<TConnection>
    {
        protected ContainerBuilder Builder { get; private set; }

        public AbstractConfigurator()
        {
            Builder = new ContainerBuilder();
        }

        public IServiceBus Bus(Action<ServiceBusConfigurer<TConnection>> action)
        {
            var serviceBusConfigurer = new ServiceBusConfigurer<TConnection>();
            action(serviceBusConfigurer);
            var serviceBusConfiguration = serviceBusConfigurer.Built;
            
            RegisterServiceBusModule(serviceBusConfiguration);
            return Builder.Build().BeginLifetimeScope("bus").Resolve<IServiceBus>();
        }


        public QueueResolver<TConnection> ConfigureQueue(Action<EndPointConfigurer<TConnection>> action)
        {
            var queueConfigurer = new EndPointConfigurer<TConnection>();
            action(queueConfigurer);
            RegisterQueueModule(queueConfigurer.Built);
            return new QueueResolver<TConnection>(Builder.Build().BeginLifetimeScope("queue").Resolve<QueueFactory>(), queueConfigurer.Built);
        }


        protected abstract void RegisterServiceBusModule(ServiceBusConfiguration<TConnection> serviceBusConfiguration);
        protected abstract void RegisterQueueModule(EndPointConfiguration<TConnection> queueConfiguration);
    }
}
