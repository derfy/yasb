using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging.Configuration;
using Autofac;
using Yasb.Common.Messaging;
using Yasb.Msmq.Messaging;
using Yasb.Common.Messaging.Configuration.Msmq;

namespace Yasb.Wireup
{

    public class MsmqConfigurator : AbstractConfigurator<MsmqConnection>
    {


        protected override void RegisterServiceBusModule(ContainerBuilder builder, ServiceBusConfiguration<MsmqConnection> serviceBusConfiguration)
        {
            builder.RegisterModule(new CommonModule<QueueConfiguration<MsmqConnection>>(serviceBusConfiguration, "bus"));
            builder.RegisterModule(new MsmqQueueModule(serviceBusConfiguration,"bus"));
            builder.RegisterModule(new MsmqServiceBusModule(serviceBusConfiguration));
        }
        protected override void RegisterQueueModule(ContainerBuilder builder, QueueConfiguration<MsmqConnection> queueConfiguration)
        {
            builder.RegisterModule(new CommonModule<QueueConfiguration<MsmqConnection>>(queueConfiguration, "queue"));
            builder.RegisterModule(new MsmqQueueModule(queueConfiguration,"queue"));
        }
       
       
    }
}
