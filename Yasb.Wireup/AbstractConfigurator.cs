using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using Yasb.Common.Messaging;
using Yasb.Common.Messaging.Configuration;
using Yasb.Common.Messaging.EndPoints;

namespace Yasb.Wireup
{
    public abstract class AbstractConfigurator<TEndPoint, TSerializerConfiguration> 
    {
        public AbstractConfigurator()
        {
        }

        public IServiceBus<TEndPoint> Bus(Action<ServiceBusConfigurer<TEndPoint, TSerializerConfiguration>> action)
        {
            var builder = new ContainerBuilder();
            var serviceBusConfigurer = new ServiceBusConfigurer<TEndPoint,TSerializerConfiguration>();
            action(serviceBusConfigurer);
            var serviceBusConfiguration = serviceBusConfigurer.Built;
            
            RegisterServiceBusModule(builder,serviceBusConfiguration);
            return builder.Build().BeginLifetimeScope("bus").Resolve<IServiceBus<TEndPoint>>();
        }


        public IQueue<TEndPoint> ConfigureQueue(Action<ServiceBusConfigurer<TEndPoint,  TSerializerConfiguration>> action)
        {
            var builder = new ContainerBuilder();
            var queueConfigurer = new ServiceBusConfigurer<TEndPoint, TSerializerConfiguration>();
            action(queueConfigurer);
            RegisterQueueModule(builder,queueConfigurer.Built);
            return builder.Build().BeginLifetimeScope("queue").Resolve<IQueue<TEndPoint>>();
        }


        protected abstract void RegisterServiceBusModule(ContainerBuilder builder, ServiceBusConfiguration<TEndPoint,TSerializerConfiguration> serviceBusConfiguration);
        protected abstract void RegisterQueueModule(ContainerBuilder builder, ServiceBusConfiguration<TEndPoint, TSerializerConfiguration> queueConfiguration);
    }
}
