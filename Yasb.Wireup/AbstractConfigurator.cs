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
        public AbstractConfigurator()
        {
        }

        public IServiceBus Bus(Action<ServiceBusConfigurer<TConnection>> action)
        {
            var builder = new ContainerBuilder();
            var serviceBusConfigurer = new ServiceBusConfigurer<TConnection>();
            action(serviceBusConfigurer);
            var serviceBusConfiguration = serviceBusConfigurer.Built;
            
            RegisterServiceBusModule(builder,serviceBusConfiguration);
            return builder.Build().BeginLifetimeScope("bus").Resolve<IServiceBus>();
        }


        public IQueueFactory ConfigureQueue(Action<QueueConfigurer<TConnection>> action)
        {
            var builder = new ContainerBuilder();
            var queueConfigurer = new QueueConfigurer<TConnection>();
            action(queueConfigurer);
            RegisterQueueModule(builder,queueConfigurer.Built);
            return builder.Build().BeginLifetimeScope("queue").Resolve<IQueueFactory>();
        }


        protected abstract void RegisterServiceBusModule(ContainerBuilder builder, ServiceBusConfiguration<TConnection> serviceBusConfiguration);
        protected abstract void RegisterQueueModule(ContainerBuilder builder, QueueConfiguration<TConnection> queueConfiguration);
    }
}
