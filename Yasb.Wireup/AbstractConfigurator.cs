using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using Yasb.Common.Messaging;
using Yasb.Common.Messaging.Configuration;
using Yasb.Common.Messaging.EndPoints;
using Autofac.Core;

namespace Yasb.Wireup
{
    public abstract class AbstractConfigurator<TEndPoint, TSubscriptionServiceConfiguration> 
    {
        public AbstractConfigurator()
        {
        }

        public IServiceBus<TEndPoint> Bus(Action<ServiceBusConfigurer<TEndPoint, TSubscriptionServiceConfiguration>> action)
        {
            var builder = new ContainerBuilder();
            var serviceBusConfigurer = new ServiceBusConfigurer<TEndPoint, TSubscriptionServiceConfiguration>();
            action(serviceBusConfigurer);
            var serviceBusConfiguration = serviceBusConfigurer.Built;
            var lifetimeScope = builder.Build().BeginLifetimeScope("bus", b => b.RegisterModule(RegisterServiceBusModule(serviceBusConfiguration)));
            return lifetimeScope.Resolve<IServiceBus<TEndPoint>>();
            
        }

       
       

        protected abstract IModule RegisterServiceBusModule(ServiceBusConfiguration<TEndPoint, TSubscriptionServiceConfiguration> serviceBusConfiguration);
    }
}
