using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging.Configuration;
using Autofac;
using Yasb.Common.Messaging;
using Yasb.Msmq.Messaging;
using Yasb.Common.Messaging.EndPoints.Msmq;
using Yasb.Common.Messaging.Configuration.Msmq;

namespace Yasb.Wireup
{

    public class MsmqConfigurator : AbstractConfigurator<MsmqEndPoint,MsmqSerializationConfiguration>
    {


        protected override void RegisterServiceBusModule(ContainerBuilder builder, ServiceBusConfiguration<MsmqEndPoint, MsmqSerializationConfiguration> serviceBusConfiguration)
        {
          //  builder.RegisterModule(new MsmqQueueModule(serviceBusConfiguration,"bus"));
            builder.RegisterModule(new MsmqServiceBusModule(serviceBusConfiguration));
        }
        protected override void RegisterQueueModule(ContainerBuilder builder, ServiceBusConfiguration<MsmqEndPoint, MsmqSerializationConfiguration> queueConfiguration)
        {
            builder.RegisterModule(new MsmqQueueModule(queueConfiguration,"queue"));
        }
       
       
    }
}
