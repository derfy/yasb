using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging.Configuration;
using Yasb.Msmq.Messaging.Configuration;
using Autofac;
using Yasb.Common.Messaging;
using Yasb.Msmq.Messaging;

namespace Yasb.Wireup
{

    public class MsmqConfigurator : AbstractConfigurator<MsmqConnection>
    {

     
        protected override void RegisterServiceBusModule(ServiceBusConfiguration<MsmqConnection> serviceBusConfiguration)
        {
            //Builder.RegisterModule(new MsmqQueueModule(serviceBusConfiguration.EndPointConfiguration,"bus"));
        }
        protected override void RegisterQueueModule(QueueConfiguration<MsmqConnection> queueConfiguration)
        {
            Builder.RegisterModule(new MsmqQueueModule(queueConfiguration,"queue"));
        }
       
       
    }
}
