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

        public IServiceBus ConfigureServiceBus(Action<ServiceBusConfigurer<MsmqConnection>> action)
        {
            var serviceBusConfigurer = new ServiceBusConfigurer<MsmqConnection>();
            action(serviceBusConfigurer);
            Builder.RegisterModule(new MsmqServiceBusModule(serviceBusConfigurer.Built));
            return Builder.Build().BeginLifetimeScope("bus").Resolve<IServiceBus>();
        }
        public QueueResolver<MsmqConnection> ConfigureQueue(Action<EndPointConfigurer<MsmqConnection>> action)
        {
            var queueConfigurer = new EndPointConfigurer<MsmqConnection>();
            action(queueConfigurer);
            Builder.RegisterModule(new MsmqQueueModule(queueConfigurer.Built));
            return new QueueResolver<MsmqConnection>(Builder.Build().BeginLifetimeScope("queue").Resolve<QueueFactory>(), queueConfigurer.Built);
        }
       
    }
}
