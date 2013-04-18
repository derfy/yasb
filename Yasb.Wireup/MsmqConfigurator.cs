using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging.Configuration;
using Yasb.Msmq.Messaging.Configuration;
using Autofac;
using Yasb.Common.Messaging;

namespace Yasb.Wireup
{
    
    public class MsmqConfigurator 
    {
        protected ContainerBuilder Builder{get;private set;}

        public MsmqConfigurator()
        {
            Builder = new ContainerBuilder();
        }

        public IServiceBus ConfigureServiceBus(Action<ServiceBusConfigurer<MsmqConnectionConfiguration>> action)
        {
            var serviceBusConfigurer = new ServiceBusConfigurer<MsmqConnectionConfiguration>();
            action(serviceBusConfigurer);
            Builder.RegisterModule(new MsmqServiceBusModule(serviceBusConfigurer.Built));
            return Builder.Build().BeginLifetimeScope("bus").Resolve<IServiceBus>();
        }
        public QueueResolver<MsmqConnectionConfiguration> ConfigureQueue(Action<EndPointConfigurer<MsmqConnectionConfiguration>> action)
        {
            var queueConfigurer = new EndPointConfigurer<MsmqConnectionConfiguration>();
            action(queueConfigurer);
            Builder.RegisterModule(new MsmqQueueModule(queueConfigurer.Built));
            return new QueueResolver<MsmqConnectionConfiguration>(Builder.Build().BeginLifetimeScope("queue").Resolve<QueueFactory>(), queueConfigurer.Built);
        }
       
    }
}
